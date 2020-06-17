﻿/// <reference path="../_ref.ts" />

module Estimatorx {
    "use strict";

    export class OrganizationEditController implements angular.IController {

        // protect for minification, must match contructor signiture.
        static $inject = [
            '$scope',
            '$uibModal',
            'logger',
            'identityService',
            'modelFactory',
            'organizationRepository',
            'inviteRepository',
            'userRepository'
        ];

        constructor(
            $scope,
            $uibModal: angular.ui.bootstrap.IModalService,
            logger: Logger,
            identityService: IdentityService,
            modelFactory: ModelFactory,
            organizationRepository: OrganizationRepository,
            inviteRepository: InviteRepository,
            userRepository: UserRepository) {

            var self = this;

            // assign viewModel to controller
            $scope.viewModel = this;
            self.$scope = $scope;
            self.$uibModal = $uibModal;

            self.logger = logger;
            self.identityService = identityService;
            self.modelFactory = modelFactory;
            self.organizationRepository = organizationRepository;
            self.inviteRepository = inviteRepository;
            self.userRepository = userRepository;

            self.organization = <IOrganization>{};

            // watch for navigation
            $(window).bind('beforeunload', () => {
                // prevent navigation by returning string
                if (self.isDirty())
                    return 'You have unsaved changes!';
            });
        }

        $scope: any;
        $uibModal: angular.ui.bootstrap.IModalService;
        
        logger: Logger;
        identityService: IdentityService;
        modelFactory: ModelFactory;

        organizationRepository: OrganizationRepository;

        original: IOrganization;
        organization: IOrganization;
        organizationId: string;

        userRepository: UserRepository;
        userId: string;

        members: IUser[];
        owners: IUser[];

        inviteRepository: InviteRepository;
        invites: IInvite[];

        isSelf(id: string): boolean {
            var self = this;
            return id === self.userId;
        }

        selfOwner(): boolean {
            var self = this;
            return self.isOwner(self.userId);
        }

        isOwner(id: string): boolean {
            var self = this;
            if (!id || !self.organization || !self.organization.Owners || self.organization.Owners.length === 0)
                return false;

            return _.contains(self.organization.Owners, id);
        }

        load(organizationId?: string, userId?: string) {
            var self = this;

            self.organizationId = organizationId;
            self.userId = userId;

            // get organization id
            if (!self.organizationId) {
                self.organization = self.modelFactory.createOrganization();
                return;
            }

            self.loadOrganization();
        }

        loadDone(organization: IOrganization) {
            var self = this;

            self.original = <IOrganization>angular.copy(organization);
            self.organization = organization;

            self.setClean();
        }

        loadOrganization() {
            var self = this;

            self.organizationRepository.find(self.organizationId)
                .then((response) => {
                    self.loadDone(response.data);

                    self.loadMembers();
                    self.loadOwners();
                    self.loadInvites();
                })
                .catch((reason) => {
                    if (reason.status === 404) {
                        self.organization = self.modelFactory.createOrganization(self.organizationId, self.userId);
                        return;
                    }

                    self.logger.handelError(reason);
                });
        }

        loadMembers() {
            var self = this;

            self.userRepository.organizationMembers(self.organizationId)
                .then((response) => {
                    self.members = response.data;
                })
                .catch(self.logger.handelErrorProxy);
        }

        loadOwners() {
            var self = this;

            self.userRepository.organizationOwners(self.organizationId)
                .then((response) => {
                    self.owners = response.data;
                })
                .catch(self.logger.handelErrorProxy);
        }

        loadInvites() {
            var self = this;

            self.inviteRepository.organization(self.organizationId)
                .then((response) => {
                    self.invites = response.data;
                })
                .catch(self.logger.handelErrorProxy);
        }

        save(valid: boolean) {
            var self = this;
                        
            if (!valid) {
                self.logger.showAlert({
                    type: 'error',
                    title: 'Validation Error',
                    message: 'A form field has a validation error. Please fix the error to continue.',
                    timeOut: 4000
                });

                return;
            }

            this.organizationRepository.save(this.organization)
                .then((response) => {
                    self.loadDone(response.data);
                    
                    self.logger.showAlert({
                        type: 'success',
                        title: 'Save Successful',
                        message: 'Organization saved successfully.',
                        timeOut: 4000
                    });

                    self.loadMembers();
                    self.loadOwners();
                    self.loadInvites();
                })
                .catch(self.logger.handelErrorProxy);
        }


        undo() {
            var self = this;

            BootstrapDialog.confirm("Are you sure you want to undo changes?", (result) => {
                if (!result)
                    return;

                self.organization = <IOrganization>angular.copy(self.original);

                self.setClean();

                self.$scope.$applyAsync();
            });
        }

        isDirty(): boolean {
            return this.$scope.organizationForm.$dirty;
        }

        setDirty() {
            this.$scope.organizationForm.$setDirty();
        }

        setClean() {
            this.$scope.organizationForm.$setPristine();
            this.$scope.organizationForm.$setUntouched();
        }


        addMember() {
            var self = this;

            var modalInstance = self.$uibModal.open({
                templateUrl: 'memberModal.html',
                controller: 'memberModalController'
            });

            modalInstance.result.then((userId: string) => {
                self.userRepository.addOrganization(self.organizationId, userId)
                    .then((response) => {
                        self.loadMembers();
                    })
                    .catch(self.logger.handelErrorProxy);
            });

        }

        removeMember(user: IUser) {
            var self = this;

            // don't remove self
            if (user.Id == self.userId)
                return;

            BootstrapDialog.confirm("Are you sure you want to remove this member?", (result) => {
                if (!result)
                    return;

                self.userRepository.removeOrganization(self.organizationId, user.Id)
                    .then((response) => {
                        self.loadMembers();
                    })
                    .catch(self.logger.handelErrorProxy);
            });
        }


        addOwner() {
            var self = this;

            var modalInstance = self.$uibModal.open({
                templateUrl: 'memberModal.html',
                controller: 'memberModalController'
            });

            modalInstance.result.then((userId: string) => {
                self.organization.Owners.push(userId);
                self.save(true);
            });
        }

        removeOwner(user: IUser) {
            var self = this;

            // don't remove self
            if (user.Id === self.userId)
                return;

            BootstrapDialog.confirm("Are you sure you want to remove this owner?", (result) => {
                if (!result)
                    return;

                for (var i = 0; i < self.organization.Owners.length; i++) {
                    if (self.organization.Owners[i] === user.Id) {
                        self.organization.Owners.splice(i, 1);
                        break;
                    }
                }

                self.save(true);
            });
        }


        addInvite() {
            var self = this;

            var modalInstance = self.$uibModal.open({
                templateUrl: 'inviteModal.html',
                controller: 'inviteModalController'
            });

            modalInstance.result.then((email: string) => {
                console.log('Invite Email: ' + email);

                var invite = <IInvite>{
                    Id: self.identityService.newObjectId(),
                    OrganizationId: self.organizationId,
                    SecurityKey: self.identityService.newSecurityKey(),
                    Email: email
                }

                self.inviteRepository.save(invite)
                    .then((response) => {
                        self.loadInvites();
                        self.sendInvite(response.data);
                    })
                    .catch(self.logger.handelErrorProxy);
            });

        }

        removeInvite(invite: IInvite) {
            var self = this;
            if (!invite)
                return;

            BootstrapDialog.confirm("Are you sure you want to remove this invite?",(result) => {
                if (!result)
                    return;

                self.inviteRepository.delete(invite.Id)
                    .then((response) => {
                        self.loadInvites();
                    })
                    .catch(self.logger.handelErrorProxy);
            });
        }

        sendInvite(invite: IInvite) {
            var self = this;
            if (!invite)
                return;

            self.inviteRepository.send(invite.Id)
                .then((response) => {
                    self.logger.showAlert({
                        type: 'success',
                        title: 'Send Successful',
                        message: 'Sent invite to ' + invite.Email + ' successfully.',
                        timeOut: 8000
                    });
                
                    self.loadInvites();
                })
                .catch(self.logger.handelErrorProxy);
        }

        $onInit = () => { };
    }

    // register controller
    angular.module(Estimatorx.applicationName)
        .controller('organizationEditController', OrganizationEditController);
}

