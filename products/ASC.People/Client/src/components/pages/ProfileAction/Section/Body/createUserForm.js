import React from "react";
import { withRouter } from "react-router";
import { connect } from "react-redux";
import {
  Avatar,
  Button,
  Textarea,
  AvatarEditor,
  Text,
  utils,
} from "asc-web-components";
import { withTranslation, Trans } from "react-i18next";
import {
  toEmployeeWrapper,
  getUserRole,
  getUserContactsPattern,
  getUserContacts,
  mapGroupsToGroupSelectorOptions,
  mapGroupSelectorOptionsToGroups,
  filterGroupSelectorOptions,
} from "../../../../../store/people/selectors";
import {
  createProfile,
  updateCreatedAvatar,
  setCreatedAvatar,
  setCroppedAvatar,
  resetProfile,
} from "../../../../../store/profile/actions";
import {
  setFilter,
  updateProfileInUsers,
  setIsVisibleDataLossDialog,
  setIsEditingForm,
  toggleAvatarEditor,
} from "../../../../../store/people/actions";
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
import { DataLossWarningDialog } from "../../../../dialogs";
import { api, toastr } from "asc-web-common";
import { isMobile } from "react-device-detect";
const { createThumbnailsAvatar, loadAvatar } = api.people;
const { isTablet } = utils.device;

class CreateUserForm extends React.Component {
  constructor(props) {
    super(props);

    this.state = this.mapPropsToState(props);

    this.validate = this.validate.bind(this);
    this.handleSubmit = this.handleSubmit.bind(this);
    this.onInputChange = this.onInputChange.bind(this);
    this.onBirthdayDateChange = this.onBirthdayDateChange.bind(this);
    this.onWorkFromDateChange = this.onWorkFromDateChange.bind(this);
    this.onCancel = this.onCancel.bind(this);
    this.onCancelHandler = this.onCancelHandler.bind(this);

    this.onContactsItemAdd = this.onContactsItemAdd.bind(this);
    this.onContactsItemTypeChange = this.onContactsItemTypeChange.bind(this);
    this.onContactsItemTextChange = this.onContactsItemTextChange.bind(this);
    this.onContactsItemRemove = this.onContactsItemRemove.bind(this);

    this.onShowGroupSelector = this.onShowGroupSelector.bind(this);
    this.onCloseGroupSelector = this.onCloseGroupSelector.bind(this);
    this.onSearchGroups = this.onSearchGroups.bind(this);
    this.onSelectGroups = this.onSelectGroups.bind(this);
    this.onRemoveGroup = this.onRemoveGroup.bind(this);

    this.openAvatarEditor = this.openAvatarEditor.bind(this);
    this.openAvatarEditorPage = this.openAvatarEditorPage.bind(this);
    this.onSaveAvatar = this.onSaveAvatar.bind(this);
    this.onCloseAvatarEditor = this.onCloseAvatarEditor.bind(this);
    this.createAvatar = this.createAvatar.bind(this);
    this.onLoadFileAvatar = this.onLoadFileAvatar.bind(this);

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
          `${this.props.settings.homepage}/view/${userName}`
        );
      })
      .catch((error) => toastr.error(error));
  }

  openAvatarEditor() {
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
  }

  openAvatarEditorPage() {
    const { toggleAvatarEditor } = this.props;

    toggleAvatarEditor(true);
  }

  onLoadFileAvatar(file, fileData) {
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
  }

  onSaveAvatar(isUpdate, result, avatar, croppedImage) {
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
  }

  onCloseAvatarEditor() {
    this.setState({
      visibleAvatarEditor: false,
      croppedAvatarImage: "",
      avatar: {
        tmpFile: "",
      },
    });
  }

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
    const { editingForm, setIsEditingForm } = this.props;
    if (!editingForm.isEdit) setIsEditingForm(true);
  }

  onInputChange(event) {
    var stateCopy = Object.assign({}, this.state);
    stateCopy.profile[event.target.name] = event.target.value;
    this.setState(stateCopy);
    this.setIsEdit();
  }

  onBirthdayDateChange(value) {
    var stateCopy = Object.assign({}, this.state);
    stateCopy.profile.birthday = value ? value.toJSON() : null;
    this.setState(stateCopy);
    this.setIsEdit();
  }

  onWorkFromDateChange(value) {
    var stateCopy = Object.assign({}, this.state);
    stateCopy.profile.workFrom = value ? value.toJSON() : null;
    this.setState(stateCopy);
    this.setIsEdit();
  }

  validate() {
    const { profile, errors: stateErrors } = this.state;
    const errors = {
      firstName: !profile.firstName.trim(),
      lastName: !profile.lastName.trim(),
      email: stateErrors.email || !profile.email.trim(),
      password: profile.passwordType === "temp" && !profile.password.trim(),
    };
    const hasError =
      errors.firstName || errors.lastName || errors.email || errors.password;

    if (hasError) {
      const element = this.mainFieldsContainerRef.current;
      const parent = element.closest(".scroll-body");
      (parent || window).scrollTo(0, element.offsetTop);
    }

    this.setState({ errors: errors });
    return !hasError;
  }

  handleSubmit() {
    if (!this.validate()) return false;
    const { setIsEditingForm } = this.props;

    this.setState({ isLoading: true });
    this.props
      .createProfile(this.state.profile)
      .then((profile) => {
        if (this.props.createdAvatar.tmpFile !== "") {
          this.createAvatar(profile.id, profile.userName);
        } else {
          toastr.success(this.props.t("ChangesSavedSuccessfully"));
          this.props.history.push(
            `${this.props.settings.homepage}/view/${profile.userName}`
          );
        }
        setIsEditingForm(false);
      })
      .catch((error) => {
        toastr.error(error);
        this.setState({ isLoading: false });
      });
  }

  onCancelHandler() {
    const { editingForm, setIsVisibleDataLossDialog } = this.props;

    if (editingForm.isEdit) {
      setIsVisibleDataLossDialog(true);
    } else {
      this.onCancel();
    }
  }

  onCancel() {
    const { filter, setFilter } = this.props;
    setFilter(filter);
  }

  onContactsItemAdd(item) {
    var stateCopy = Object.assign({}, this.state);
    stateCopy.profile.contacts.push({
      id: new Date().getTime().toString(),
      type: item.value,
      value: "",
    });
    this.setState(stateCopy);
    this.setIsEdit();
  }

  onContactsItemTypeChange(item) {
    const id = item.key.split("_")[0];
    var stateCopy = Object.assign({}, this.state);
    stateCopy.profile.contacts.forEach((element) => {
      if (element.id === id) element.type = item.value;
    });
    this.setState(stateCopy);
    this.setIsEdit();
  }

  onContactsItemTextChange(event) {
    const id = event.target.name.split("_")[0];
    var stateCopy = Object.assign({}, this.state);
    stateCopy.profile.contacts.forEach((element) => {
      if (element.id === id) element.value = event.target.value;
    });
    this.setState(stateCopy);
    this.setIsEdit();
  }

  onContactsItemRemove(event) {
    const id = event.target.closest(".remove_icon").dataset.for.split("_")[0];
    var stateCopy = Object.assign({}, this.state);
    const filteredArray = stateCopy.profile.contacts.filter((element) => {
      return element.id !== id;
    });
    stateCopy.profile.contacts = filteredArray;
    this.setState(stateCopy);
    this.setIsEdit();
  }

  onShowGroupSelector() {
    var stateCopy = Object.assign({}, this.state);
    stateCopy.selector.visible = true;
    this.setState(stateCopy);
  }

  onCloseGroupSelector() {
    var stateCopy = Object.assign({}, this.state);
    stateCopy.selector.visible = false;
    this.setState(stateCopy);
  }

  onSearchGroups(template) {
    var stateCopy = Object.assign({}, this.state);
    stateCopy.selector.options = filterGroupSelectorOptions(
      stateCopy.selector.allOptions,
      template
    );
    this.setState(stateCopy);
  }

  onSelectGroups(selected) {
    var stateCopy = Object.assign({}, this.state);
    stateCopy.profile.groups = mapGroupSelectorOptionsToGroups(selected);
    stateCopy.selector.selected = selected;
    stateCopy.selector.visible = false;
    this.setState(stateCopy);
    this.setIsEdit();
  }

  onRemoveGroup(id) {
    var stateCopy = Object.assign({}, this.state);
    stateCopy.profile.groups = stateCopy.profile.groups.filter(
      (group) => group.id !== id
    );
    stateCopy.selector.selected = stateCopy.selector.selected.filter(
      (option) => option.key !== id
    );
    this.setState(stateCopy);
  }

  onValidateEmailField = (value) =>
    this.setState({ errors: { ...this.state.errors, email: !value.isValid } });

  render() {
    const { isLoading, errors, profile, selector, isMobile } = this.state;
    const { t, settings, i18n, createdAvatar, croppedAvatar } = this.props;
    const {
      regDateCaption,
      userPostCaption,
      groupCaption,
    } = settings.customNames;

    const pattern = getUserContactsPattern();
    const contacts = getUserContacts(profile.contacts);

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
              editLabel={t("AddButton")}
              editAction={
                isMobile ? this.openAvatarEditorPage : this.openAvatarEditor
              }
            />
            <AvatarEditor
              image={createdAvatar.image}
              visible={this.state.visibleAvatarEditor}
              onClose={this.onCloseAvatarEditor}
              onSave={this.onSaveAvatar}
              onLoadFile={this.onLoadFileAvatar}
              headerLabel={t("AddPhoto")}
              selectNewPhotoLabel={t("selectNewPhotoLabel")}
              orDropFileHereLabel={t("orDropFileHereLabel")}
              unknownTypeError={t("ErrorUnknownFileImageType")}
              maxSizeFileError={t("maxSizeFileError")}
              unknownError={t("Error")}
              saveButtonLabel={t("SaveButton")}
            />
          </AvatarContainer>
          <MainFieldsContainer ref={this.mainFieldsContainerRef}>
            <TextField
              isRequired={true}
              hasError={errors.firstName}
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
              labelText={`${t("LastName")}:`}
              inputName="lastName"
              inputValue={profile.lastName}
              inputIsDisabled={isLoading}
              inputOnChange={this.onInputChange}
              inputTabIndex={2}
            />
            <EmailField
              isRequired={true}
              hasError={errors.email}
              labelText={`${t("Email")}:`}
              inputName="email"
              inputValue={profile.email}
              inputIsDisabled={isLoading}
              inputOnChange={this.onInputChange}
              inputTabIndex={3}
              helpButtonHeaderContent={t("Mail")}
              tooltipContent={
                <Text fontSize="13px" as="div">
                  <Trans i18nKey="EmailPopupHelper" i18n={i18n}>
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
              labelText={`${t("Password")}:`}
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
              copyLinkText={t("CopyEmailAndPassword")}
              inputTabIndex={4}
              passwordSettings={settings.passwordSettings}
              t={t}
            />
            <DateField
              calendarHeaderContent={`${t("CalendarSelectDate")}:`}
              labelText={`${t("Birthdate")}:`}
              inputName="birthday"
              inputValue={
                profile.birthday ? new Date(profile.birthday) : undefined
              }
              inputIsDisabled={isLoading}
              inputOnChange={this.onBirthdayDateChange}
              inputTabIndex={5}
            />
            <RadioField
              labelText={`${t("Sex")}:`}
              radioName="sex"
              radioValue={profile.sex}
              radioOptions={[
                { value: "male", label: t("MaleSexStatus") },
                { value: "female", label: t("FemaleSexStatus") },
              ]}
              radioIsDisabled={isLoading}
              radioOnChange={this.onInputChange}
            />
            <DateField
              calendarHeaderContent={`${t("CalendarSelectDate")}:`}
              labelText={`${regDateCaption}:`}
              inputName="workFrom"
              inputValue={
                profile.workFrom ? new Date(profile.workFrom) : undefined
              }
              inputIsDisabled={isLoading}
              inputOnChange={this.onWorkFromDateChange}
              inputTabIndex={6}
            />
            <TextField
              labelText={`${t("Location")}:`}
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
              showGroupSelectorButtonTitle={t("AddButton")}
              onShowGroupSelector={this.onShowGroupSelector}
              onCloseGroupSelector={this.onCloseGroupSelector}
              onRemoveGroup={this.onRemoveGroup}
              selectorIsVisible={selector.visible}
              selectorOptions={selector.options}
              selectorSelectedOptions={selector.selected}
              selectorSelectAllText={t("SelectAll")}
              selectorOnSearchGroups={this.onSearchGroups}
              selectorOnSelectGroups={this.onSelectGroups}
            />
          </MainFieldsContainer>
        </MainContainer>
        <InfoFieldContainer headerText={t("Comments")}>
          <Textarea
            placeholder={t("WriteComment")}
            name="notes"
            value={profile.notes}
            isDisabled={isLoading}
            onChange={this.onInputChange}
            tabIndex={9}
          />
        </InfoFieldContainer>
        <InfoFieldContainer headerText={t("ContactInformation")}>
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
        <InfoFieldContainer headerText={t("SocialProfiles")}>
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
            label={t("SaveButton")}
            onClick={this.handleSubmit}
            primary
            isDisabled={isLoading}
            size="big"
            tabIndex={10}
          />
          <Button
            label={t("CancelButton")}
            onClick={this.onCancelHandler}
            isDisabled={isLoading}
            size="big"
            style={{ marginLeft: "8px" }}
            tabIndex={11}
          />
        </div>
      </>
    );
  }
}

const mapStateToProps = (state) => {
  const { settings } = state.auth;
  const { groups, filter, editingForm } = state.people;
  const { createdAvatar, croppedAvatar } = state.profile;
  return {
    settings,
    groups,
    filter,
    editingForm,
    createdAvatar,
    croppedAvatar,
  };
};

export default connect(mapStateToProps, {
  createProfile,
  updateCreatedAvatar,
  setFilter,
  updateProfileInUsers,
  setIsVisibleDataLossDialog,
  setIsEditingForm,
  toggleAvatarEditor,
  setCreatedAvatar,
  setCroppedAvatar,
  resetProfile,
})(withRouter(withTranslation()(CreateUserForm)));
