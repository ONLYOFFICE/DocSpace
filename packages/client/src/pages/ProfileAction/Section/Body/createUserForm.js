import React from "react";
import { withRouter } from "react-router";

import Avatar from "@docspace/components/avatar";
import Button from "@docspace/components/button";
import Textarea from "@docspace/components/textarea";
import Text from "@docspace/components/text";
import AvatarEditor from "@docspace/components/avatar-editor";
import { isTablet } from "@docspace/components/utils/device";

import { withTranslation, Trans } from "react-i18next";
import {
  MainContainer,
  AvatarContainer,
  MainFieldsContainer,
} from "./FormFields/Form";
import TextField from "./FormFields/TextField";
import PasswordField from "./FormFields/PasswordField";
import EmailField from "./FormFields/EmailField";
import DateField from "./FormFields/DateField";
import RadioField from "./FormFields/RadioField";
import DepartmentField from "./FormFields/DepartmentField";
import ContactsField from "./FormFields/ContactsField";
import InfoFieldContainer from "./FormFields/InfoFieldContainer";
import { DataLossWarningDialog } from "../../../../components/dialogs";
import {
  createThumbnailsAvatar,
  loadAvatar,
} from "@docspace/common/api/people";
import toastr from "@docspace/components/toast/toastr";
import { isMobile } from "react-device-detect";
import { inject, observer } from "mobx-react";
import {
  toEmployeeWrapper,
  getUserRole,
  getUserContactsPattern,
  getUserContacts,
  mapGroupsToGroupSelectorOptions,
  mapGroupSelectorOptionsToGroups,
  filterGroupSelectorOptions,
} from "../../../../helpers/people-helpers";
import config from "PACKAGE_FILE";
import { combineUrl } from "@docspace/common/utils";
import { AppServerConfig } from "@docspace/common/constants";

class CreateUserForm extends React.Component {
  constructor(props) {
    super(props);

    this.state = this.mapPropsToState(props);
    this.mainFieldsContainerRef = React.createRef();
  }

  createAvatar(userId, userName) {
    const { createdAvatar } = this.props;
    createThumbnailsAvatar(userId, {
      x: createdAvatar.x,
      y: createdAvatar.y,
      width: createdAvatar.width,
      height: createdAvatar.height,
      tmpFile: createdAvatar.tmpFile,
    })
      .then((res) => {
        this.props.updateCreatedAvatar(res);
        this.props.updateProfileInUsers();
        toastr.success(this.props.t("ChangesSavedSuccessfully"));
        this.props.history.push(
          combineUrl(
            AppServerConfig.proxyURL,
            this.props.homepage,
            `/accounts/view/${userName}`
          )
        );
      })
      .catch((error) => toastr.error(error));
  }

  openAvatarEditor = () => {
    let avatarDefault = this.state.avatar.image;
    let avatarDefaultSizes = /_orig_(\d*)-(\d*)./g.exec(
      this.state.avatar.image
    );
    if (
      avatarDefault !== null &&
      avatarDefaultSizes !== null &&
      avatarDefaultSizes.length > 2
    ) {
      this.setState({
        avatar: {
          tmpFile: this.state.avatar.tmpFile,
          image: this.state.avatar.image,
          defaultWidth: avatarDefaultSizes[1],
          defaultHeight: avatarDefaultSizes[2],
        },
      });
    }
    this.setState({
      visibleAvatarEditor: true,
    });
  };

  openAvatarEditorPage = () => {
    const { toggleAvatarEditor } = this.props;

    toggleAvatarEditor(true);
  };

  onLoadFileAvatar = (file, fileData) => {
    let data = new FormData();
    let _this = this;
    data.append("file", file);
    data.append("Autosave", false);

    if (!file) {
      _this.onSaveAvatar(false);
      return;
    }

    loadAvatar(0, data)
      .then((response) => {
        if (!response.success && response.message) {
          throw response.message;
        }
        var img = new Image();
        img.onload = function () {
          if (fileData) {
            fileData.avatar = {
              tmpFile: response.data,
              image: response.data,
              defaultWidth: img.width,
              defaultHeight: img.height,
            };

            var stateCopy = Object.assign({}, _this.state);
            stateCopy.avatar = {
              tmpFile: response.data,
              image: response.data,
              defaultWidth: img.width,
              defaultHeight: img.height,
            };
            _this.setState(stateCopy);

            if (fileData.existImage) {
              _this.onSaveAvatar(
                fileData.existImage,
                fileData.position,
                fileData.avatar,
                fileData.croppedImage
              );
            }
          }
        };
        img.src = response.data;
      })
      .catch((error) => toastr.error(error));
  };

  onSaveAvatar = (isUpdate, result, avatar, croppedImage) => {
    var stateCopy = Object.assign({}, this.state);
    const { setCreatedAvatar, setCroppedAvatar, resetProfile } = this.props;

    stateCopy.visibleAvatarEditor = false;
    stateCopy.croppedAvatarImage = croppedImage;
    if (isUpdate) {
      stateCopy.avatar.x = Math.round(
        result.x * avatar.defaultWidth - result.width / 2
      );
      stateCopy.avatar.y = Math.round(
        result.y * avatar.defaultHeight - result.height / 2
      );
      stateCopy.avatar.width = result.width;
      stateCopy.avatar.height = result.height;

      setCreatedAvatar(stateCopy.avatar);
      setCroppedAvatar(croppedImage);
    } else {
      resetProfile();
    }
    this.setState(stateCopy);
    this.setIsEdit();
  };

  onCloseAvatarEditor = () => {
    this.setState({
      visibleAvatarEditor: false,
      croppedAvatarImage: "",
      avatar: {
        tmpFile: "",
      },
    });
  };

  componentDidUpdate(prevProps, prevState) {
    if (this.props.match.params.type !== prevProps.match.params.type) {
      this.setState(this.mapPropsToState(this.props));
    }

    const isMobileDevice = isMobile || isTablet();

    if (prevState.isMobile !== isMobileDevice) {
      this.setState({ isMobile: isMobileDevice });
    }
  }

  mapPropsToState = (props) => {
    var profile = toEmployeeWrapper({
      isVisitor: props.match.params.type === "guest",
      passwordType: "link",
    });
    var allOptions = mapGroupsToGroupSelectorOptions(props.groups);
    var selected = mapGroupsToGroupSelectorOptions(profile.groups);
    return {
      visibleAvatarEditor: false,
      croppedAvatarImage: "",
      isLoading: false,
      errors: {
        firstName: false,
        lastName: false,
        email: false,
        password: false,
      },
      profile: profile,
      selector: {
        visible: false,
        allOptions: allOptions,
        options: [...allOptions],
        selected: selected,
      },
      avatar: {
        tmpFile: "",
        image: null,
        defaultWidth: 0,
        defaultHeight: 0,
        x: 0,
        y: 0,
        width: 0,
        height: 0,
      },
      isMobile: isMobile || isTablet,
    };
  };
  setIsEdit() {
    const { isEdit, setIsEditingForm } = this.props;
    if (!isEdit) setIsEditingForm(true);
  }

  onInputChange = (event) => {
    const { userFormValidation } = this.props;
    var stateCopy = Object.assign({}, this.state);
    const value = event.target.value;
    const title = event.target.name;

    if (!value.match(userFormValidation)) {
      stateCopy.errors[title] = true;
    } else {
      if (this.state.errors[title]) stateCopy.errors[title] = false;
    }

    stateCopy.profile[title] = value;

    this.setState(stateCopy);
    this.setIsEdit();
  };

  onBirthdayDateChange = (value) => {
    var stateCopy = Object.assign({}, this.state);
    stateCopy.profile.birthday = value ? value.toJSON() : null;
    this.setState(stateCopy);
    this.setIsEdit();
  };

  onWorkFromDateChange = (value) => {
    var stateCopy = Object.assign({}, this.state);
    stateCopy.profile.workFrom = value ? value.toJSON() : null;
    this.setState(stateCopy);
    this.setIsEdit();
  };

  scrollToErrorForm = () => {
    const element = this.mainFieldsContainerRef.current;
    const parent = element.closest(".scroll-body");
    (parent || window).scrollTo(0, element.offsetTop);
  };

  validate = () => {
    const { profile, errors: stateErrors } = this.state;

    if (stateErrors.firstName || stateErrors.lastName) {
      this.scrollToErrorForm();
      return;
    }

    const errors = {
      firstName: !profile.firstName.trim(),
      lastName: !profile.lastName.trim(),
      email: stateErrors.email || !profile.email.trim(),
      password: profile.passwordType === "temp" && !profile.password.trim(),
    };
    const hasError =
      errors.firstName || errors.lastName || errors.email || errors.password;

    if (hasError) {
      this.scrollToErrorForm();
    }

    this.setState({ errors: errors });
    return !hasError;
  };

  handleSubmit = () => {
    if (!this.validate()) return false;
    const {
      setIsEditingForm,
      homepage,
      createProfile,
      createdAvatar,
      t,
      history,
    } = this.props;
    const profile = this.state.profile;
    if (!profile.birthday) profile.birthday = new Date();

    this.setState({ isLoading: true });
    createProfile(profile)
      .then((profile) => {
        if (createdAvatar.tmpFile !== "") {
          this.createAvatar(profile.id, profile.userName);
        } else {
          toastr.success(t("ChangesSavedSuccessfully"));
          history.push(
            combineUrl(
              AppServerConfig.proxyURL,
              homepage,
              `/accounts/view/${profile.userName}`
            )
          );
        }
        setIsEditingForm(false);
      })
      .catch((error) => {
        toastr.error(error);
        this.setState({ isLoading: false });
      });
  };

  onCancelHandler = () => {
    const { isEdit, setIsVisibleDataLossDialog } = this.props;

    if (isEdit) {
      setIsVisibleDataLossDialog(true);
    } else {
      this.onCancel();
    }
  };

  onCancel = () => {
    const { filter, setFilter, history } = this.props;
    history.goBack();
    setFilter(filter);
  };

  onContactsItemAdd = (item) => {
    var stateCopy = Object.assign({}, this.state);
    stateCopy.profile.contacts.push({
      id: new Date().getTime().toString(),
      type: item.value,
      value: "",
    });
    this.setState(stateCopy);
    this.setIsEdit();
  };

  onContactsItemTypeChange = (item) => {
    const id = item.key.split("_")[0];
    var stateCopy = Object.assign({}, this.state);
    stateCopy.profile.contacts.forEach((element) => {
      if (element.id === id) element.type = item.value;
    });
    this.setState(stateCopy);
    this.setIsEdit();
  };

  onContactsItemTextChange = (event) => {
    const id = event.target.name.split("_")[0];
    var stateCopy = Object.assign({}, this.state);
    stateCopy.profile.contacts.forEach((element) => {
      if (element.id === id) element.value = event.target.value;
    });
    this.setState(stateCopy);
    this.setIsEdit();
  };

  onContactsItemRemove = (event) => {
    const id = event.target.closest(".remove_icon").dataset.for.split("_")[0];
    var stateCopy = Object.assign({}, this.state);
    const filteredArray = stateCopy.profile.contacts.filter((element) => {
      return element.id !== id;
    });
    stateCopy.profile.contacts = filteredArray;
    this.setState(stateCopy);
    this.setIsEdit();
  };

  onShowGroupSelector = () => {
    var stateCopy = Object.assign({}, this.state);
    stateCopy.selector.visible = true;
    this.setState(stateCopy);
  };

  onCloseGroupSelector = () => {
    var stateCopy = Object.assign({}, this.state);
    stateCopy.selector.visible = false;
    this.setState(stateCopy);
  };

  onSearchGroups = (template) => {
    var stateCopy = Object.assign({}, this.state);
    stateCopy.selector.options = filterGroupSelectorOptions(
      stateCopy.selector.allOptions,
      template
    );
    this.setState(stateCopy);
  };

  onSelectGroups = (selected) => {
    var stateCopy = Object.assign({}, this.state);
    stateCopy.profile.groups = mapGroupSelectorOptionsToGroups(selected);
    stateCopy.selector.selected = selected;
    stateCopy.selector.visible = false;
    this.setState(stateCopy);
    this.setIsEdit();
  };

  onRemoveGroup = (id) => {
    var stateCopy = Object.assign({}, this.state);
    stateCopy.profile.groups = stateCopy.profile.groups.filter(
      (group) => group.id !== id
    );
    stateCopy.selector.selected = stateCopy.selector.selected.filter(
      (option) => option.key !== id
    );
    this.setState(stateCopy);
  };

  onValidateEmailField = (value) =>
    this.setState({ errors: { ...this.state.errors, email: !value.isValid } });

  onSaveClick = () => this.setState({ isLoading: true });

  render() {
    const { isLoading, errors, profile, selector, isMobile } = this.state;
    const {
      t,
      customNames,
      createdAvatar,
      croppedAvatar,
      passwordSettings,
      language,
      isTabletView,
    } = this.props;
    const { regDateCaption, userPostCaption, groupCaption } = customNames;

    const pattern = getUserContactsPattern();
    const contacts = getUserContacts(profile.contacts);

    const notEmptyFirstName = Boolean(profile.firstName.trim());
    const notEmptyLastName = Boolean(profile.lastName.trim());

    return (
      <>
        <MainContainer>
          <DataLossWarningDialog onContinue={this.onCancel} />
          <AvatarContainer>
            <Avatar
              size="max"
              role={getUserRole(profile)}
              editing={true}
              source={croppedAvatar}
              editLabel={t("Common:AddButton")}
              editAction={
                isMobile ? this.openAvatarEditorPage : this.openAvatarEditor
              }
            />
            <AvatarEditor
              image={createdAvatar.image}
              visible={this.state.visibleAvatarEditor}
              onClose={this.onCloseAvatarEditor}
              onSave={this.onSaveClick}
              onLoadFile={this.onLoadFileAvatar}
              headerLabel={t("AddPhoto")}
              selectNewPhotoLabel={t("PeopleTranslations:selectNewPhotoLabel")}
              orDropFileHereLabel={t("PeopleTranslations:orDropFileHereLabel")}
              unknownTypeError={t(
                "PeopleTranslations:ErrorUnknownFileImageType"
              )}
              maxSizeFileError={t("PeopleTranslations:maxSizeFileError")}
              unknownError={t("Common:Error")}
              saveButtonLabel={t("Common:SaveButton")}
              saveButtonLoading={this.state.isLoading}
              maxSizeLabel={t("PeopleTranslations:MaxSizeLabel")}
            />
          </AvatarContainer>
          <MainFieldsContainer
            ref={this.mainFieldsContainerRef}
            {...(!isTabletView && { marginBottom: "32px" })}
          >
            <TextField
              isRequired={true}
              hasError={errors.firstName}
              {...(notEmptyFirstName && {
                errorMessage: t("ErrorInvalidUserFirstName"),
              })}
              labelText={`${t("FirstName")}:`}
              inputName="firstName"
              inputValue={profile.firstName}
              inputIsDisabled={isLoading}
              inputOnChange={this.onInputChange}
              inputAutoFocussed={true}
              inputTabIndex={1}
            />
            <TextField
              isRequired={true}
              hasError={errors.lastName}
              {...(notEmptyLastName && {
                errorMessage: t("ErrorInvalidUserLastName"),
              })}
              labelText={`${t("Common:LastName")}:`}
              inputName="lastName"
              inputValue={profile.lastName}
              inputIsDisabled={isLoading}
              inputOnChange={this.onInputChange}
              inputTabIndex={2}
            />
            <EmailField
              isRequired={true}
              hasError={errors.email}
              labelText={`${t("Common:Email")}:`}
              inputName="email"
              inputValue={profile.email}
              inputIsDisabled={isLoading}
              inputOnChange={this.onInputChange}
              inputTabIndex={3}
              helpButtonHeaderContent={t("Common:Mail")}
              tooltipContent={
                <Text fontSize="13px" as="div">
                  <Trans t={t} i18nKey="EmailPopupHelper" ns="ProfileAction">
                    The main e-mail is needed to restore access to the portal in
                    case of loss of the password and send notifications.
                    <p className="tooltip_email" style={{ margin: "1rem 0" }}>
                      You can create a new mail on the domain as the primary. In
                      this case, you must set a one-time password so that the
                      user can log in to the portal for the first time.
                    </p>
                    The main e-mail can be used as a login when logging in to
                    the portal.
                  </Trans>
                </Text>
              }
              onValidateInput={this.onValidateEmailField}
            />
            <PasswordField
              isRequired={true}
              hasError={errors.password}
              labelText={`${t("Common:Password")}:`}
              radioName="passwordType"
              radioValue={profile.passwordType}
              radioOptions={[
                { value: "link", label: t("ActivationLink") },
                { value: "temp", label: t("TemporaryPassword") },
              ]}
              radioIsDisabled={isLoading}
              radioOnChange={this.onInputChange}
              inputName="password"
              emailInputName="email"
              inputValue={profile.password}
              inputIsDisabled={isLoading || profile.passwordType === "link"}
              inputOnChange={this.onInputChange}
              copyLinkText={t("Common:CopyEmailAndPassword")}
              copiedResourceText={t("CopiedResourceText")}
              inputTabIndex={4}
              passwordSettings={passwordSettings}
              t={t}
            />
            <DateField
              calendarHeaderContent={`${t("CalendarSelectDate")}:`}
              labelText={`${t("PeopleTranslations:Birthdate")}:`}
              inputName="birthday"
              inputClassName="date-picker_input-birthday"
              inputValue={
                profile.birthday ? new Date(profile.birthday) : undefined
              }
              inputIsDisabled={isLoading}
              inputOnChange={this.onBirthdayDateChange}
              inputTabIndex={5}
              locale={language}
            />
            <RadioField
              labelText={`${t("PeopleTranslations:Sex")}:`}
              radioName="sex"
              radioValue={profile.sex}
              radioOptions={[
                { value: "male", label: t("PeopleTranslations:MaleSexStatus") },
                {
                  value: "female",
                  label: t("PeopleTranslations:FemaleSexStatus"),
                },
              ]}
              radioIsDisabled={isLoading}
              radioOnChange={this.onInputChange}
            />
            <DateField
              calendarHeaderContent={`${t("CalendarSelectDate")}:`}
              labelText={`${regDateCaption}:`}
              inputName="workFrom"
              inputClassName="date-picker_input-reg-date"
              inputValue={
                profile.workFrom ? new Date(profile.workFrom) : undefined
              }
              inputIsDisabled={isLoading}
              inputOnChange={this.onWorkFromDateChange}
              inputTabIndex={6}
              calendarMinDate={
                profile.birthday ? new Date(profile.birthday) : undefined
              }
              locale={language}
            />
            <TextField
              labelText={`${t("Common:Location")}:`}
              inputName="location"
              inputValue={profile.location}
              inputIsDisabled={isLoading}
              inputOnChange={this.onInputChange}
              inputTabIndex={7}
            />
            <TextField
              labelText={`${userPostCaption}:`}
              inputName="title"
              inputValue={profile.title}
              inputIsDisabled={isLoading}
              inputOnChange={this.onInputChange}
              inputTabIndex={8}
            />
            <DepartmentField
              labelText={`${groupCaption}:`}
              isDisabled={isLoading}
              showGroupSelectorButtonTitle={t("Common:AddButton")}
              onShowGroupSelector={this.onShowGroupSelector}
              onCloseGroupSelector={this.onCloseGroupSelector}
              onRemoveGroup={this.onRemoveGroup}
              selectorIsVisible={selector.visible}
              selectorOptions={selector.options}
              selectorSelectedOptions={selector.selected}
              selectorSelectAllText={t("Common:SelectAll")}
              selectorOnSearchGroups={this.onSearchGroups}
              selectorOnSelectGroups={this.onSelectGroups}
            />
          </MainFieldsContainer>
        </MainContainer>
        <InfoFieldContainer
          headerText={t("Common:Comments")}
          marginBottom={"42px"}
        >
          <Textarea
            placeholder={t("WriteComment")}
            name="notes"
            value={profile.notes}
            isDisabled={isLoading}
            onChange={this.onInputChange}
            tabIndex={9}
          />
        </InfoFieldContainer>
        <InfoFieldContainer
          headerText={t("ContactInformation")}
          marginBottom={"42px"}
        >
          <ContactsField
            pattern={pattern.contact}
            contacts={contacts.contact}
            isDisabled={isLoading}
            addItemText={t("AddContact")}
            onItemAdd={this.onContactsItemAdd}
            onItemTypeChange={this.onContactsItemTypeChange}
            onItemTextChange={this.onContactsItemTextChange}
            onItemRemove={this.onContactsItemRemove}
          />
        </InfoFieldContainer>
        <InfoFieldContainer
          headerText={t("PeopleTranslations:SocialProfiles")}
          {...(isTabletView && { marginBottom: "36px" })}
        >
          <ContactsField
            pattern={pattern.social}
            contacts={contacts.social}
            isDisabled={isLoading}
            addItemText={t("AddContact")}
            onItemAdd={this.onContactsItemAdd}
            onItemTypeChange={this.onContactsItemTypeChange}
            onItemTextChange={this.onContactsItemTextChange}
            onItemRemove={this.onContactsItemRemove}
          />
        </InfoFieldContainer>
        <div>
          <Button
            label={t("Common:SaveButton")}
            onClick={this.handleSubmit}
            primary
            isDisabled={isLoading}
            size="normal"
            tabIndex={10}
            className="create-user_save-btn"
          />
          <Button
            label={t("Common:CancelButton")}
            onClick={this.onCancelHandler}
            isDisabled={isLoading}
            size="normal"
            style={{ marginLeft: "8px" }}
            tabIndex={11}
            className="create-user_cancel-btn"
          />
        </div>
      </>
    );
  }
}

export default withRouter(
  inject(({ auth, peopleStore }) => ({
    passwordSettings: auth.settingsStore.passwordSettings,
    customNames: auth.settingsStore.customNames,
    language: auth.language,
    homepage: config.homepage,
    isEdit: peopleStore.editingFormStore.isEdit,
    groups: peopleStore.groupsStore.groups,
    setIsVisibleDataLossDialog:
      peopleStore.editingFormStore.setIsVisibleDataLossDialog,
    setIsEditingForm: peopleStore.editingFormStore.setIsEditingForm,
    filter: peopleStore.filterStore.filter,
    setFilter: peopleStore.filterStore.setFilterParams,
    toggleAvatarEditor: peopleStore.avatarEditorStore.toggleAvatarEditor,
    resetProfile: peopleStore.targetUserStore.resetTargetUser,
    createProfile: peopleStore.usersStore.createUser,
    createdAvatar: peopleStore.avatarEditorStore.createdAvatar,
    setCreatedAvatar: peopleStore.avatarEditorStore.setCreatedAvatar,
    croppedAvatar: peopleStore.avatarEditorStore.croppedAvatar,
    setCroppedAvatar: peopleStore.avatarEditorStore.setCroppedAvatar,
    updateProfileInUsers: peopleStore.usersStore.updateProfileInUsers,
    updateCreatedAvatar: peopleStore.targetUserStore.updateCreatedAvatar,
    userFormValidation: auth.settingsStore.userFormValidation,
    isTabletView: auth.settingsStore.isTabletView,
  }))(
    observer(
      withTranslation(["ProfileAction", "Common", "PeopleTranslations"])(
        CreateUserForm
      )
    )
  )
);
