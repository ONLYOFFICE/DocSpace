import React from "react";
import { withRouter } from "react-router";
import { connect } from "react-redux";
import {
  Avatar,
  Button,
  Textarea,
  Text,
  AvatarEditor,
  Link,
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
  updateProfile,
  getUserPhoto,
  fetchProfile,
  setAvatarMax,
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
import TextChangeField from "./FormFields/TextChangeField";
import DateField from "./FormFields/DateField";
import RadioField from "./FormFields/RadioField";
import DepartmentField from "./FormFields/DepartmentField";
import ContactsField from "./FormFields/ContactsField";
import InfoFieldContainer from "./FormFields/InfoFieldContainer";
import styled from "styled-components";
import { DataLossWarningDialog } from "../../../../dialogs";
import { api, toastr } from "asc-web-common";
import {
  ChangeEmailDialog,
  ChangePasswordDialog,
  ChangePhoneDialog,
} from "../../../../dialogs";
import { isMobile } from "react-device-detect";
const { createThumbnailsAvatar, loadAvatar, deleteAvatar } = api.people;
const { isTablet } = utils.device;

const dialogsDataset = {
  changeEmail: "changeEmail",
  changePassword: "changePassword",
  changePhone: "changePhone",
};

const Table = styled.table`
  width: 100%;
  margin-bottom: 23px;
`;

const Th = styled.th`
  padding: 11px 0 10px 0px;
  border-top: 1px solid #eceef1;
`;

const Td = styled.td``;

class UpdateUserForm extends React.Component {
  constructor(props) {
    super(props);

    this.state = this.mapPropsToState(props);

    this.validate = this.validate.bind(this);
    this.handleSubmit = this.handleSubmit.bind(this);
    this.onInputChange = this.onInputChange.bind(this);
    this.onUserTypeChange = this.onUserTypeChange.bind(this);
    this.onBirthdayDateChange = this.onBirthdayDateChange.bind(this);
    this.onWorkFromDateChange = this.onWorkFromDateChange.bind(this);
    this.onCancel = this.onCancel.bind(this);
    this.onCancelHandler = this.onCancelHandler.bind(this);

    this.onContactsItemAdd = this.onContactsItemAdd.bind(this);
    this.onContactsItemTypeChange = this.onContactsItemTypeChange.bind(this);
    this.onContactsItemTextChange = this.onContactsItemTextChange.bind(this);
    this.onContactsItemRemove = this.onContactsItemRemove.bind(this);

    this.openAvatarEditor = this.openAvatarEditor.bind(this);
    this.openAvatarEditorPage = this.openAvatarEditorPage.bind(this);
    this.onSaveAvatar = this.onSaveAvatar.bind(this);
    this.onCloseAvatarEditor = this.onCloseAvatarEditor.bind(this);
    this.onLoadFileAvatar = this.onLoadFileAvatar.bind(this);

    this.onShowGroupSelector = this.onShowGroupSelector.bind(this);
    this.onCloseGroupSelector = this.onCloseGroupSelector.bind(this);
    this.onSearchGroups = this.onSearchGroups.bind(this);
    this.onSelectGroups = this.onSelectGroups.bind(this);
    this.onRemoveGroup = this.onRemoveGroup.bind(this);

    this.mainFieldsContainerRef = React.createRef();
  }

  componentDidUpdate(prevProps, prevState) {
    if (this.props.match.params.userId !== prevProps.match.params.userId) {
      this.setState(this.mapPropsToState(this.props));
    }

    const isMobileDevice = isMobile || isTablet();

    if (prevState.isMobile !== isMobileDevice) {
      this.setState({ isMobile: isMobileDevice });
    }
  }

  updateUserPhotoInState = () => {
    var profile = toEmployeeWrapper(this.props.profile);
    getUserPhoto(profile.id).then((userPhotoData) => {
      if (userPhotoData.original) {
        let avatarDefaultSizes = /_(\d*)-(\d*)./g.exec(userPhotoData.original);
        if (avatarDefaultSizes !== null && avatarDefaultSizes.length > 2) {
          this.setState({
            avatar: {
              tmpFile: this.state.avatar.tmpFile,
              defaultWidth: avatarDefaultSizes[1],
              defaultHeight: avatarDefaultSizes[2],
              image: userPhotoData.original
                ? userPhotoData.original.indexOf("default_user_photo") !== -1
                  ? null
                  : userPhotoData.original
                : null,
            },
          });
        }
      }
    });
  };

  mapPropsToState = (props) => {
    var profile = toEmployeeWrapper(props.profile);
    var allOptions = mapGroupsToGroupSelectorOptions(props.groups);
    var selected = mapGroupsToGroupSelectorOptions(profile.groups);

    getUserPhoto(profile.id).then((userPhotoData) => {
      if (userPhotoData.original) {
        let avatarDefaultSizes = /_(\d*)-(\d*)./g.exec(userPhotoData.original);
        if (avatarDefaultSizes !== null && avatarDefaultSizes.length > 2) {
          this.setState({
            avatar: {
              tmpFile: this.state.avatar.tmpFile,
              defaultWidth: avatarDefaultSizes[1],
              defaultHeight: avatarDefaultSizes[2],
              image: userPhotoData.original
                ? userPhotoData.original.indexOf("default_user_photo") !== -1
                  ? null
                  : userPhotoData.original
                : null,
            },
          });
        }
      }
    });

    const newState = {
      isLoading: false,
      errors: {
        firstName: false,
        lastName: false,
      },
      profile: profile,
      visibleAvatarEditor: false,
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
      },
      dialogsVisible: {
        [dialogsDataset.changePassword]: false,
        [dialogsDataset.changePhone]: false,
        [dialogsDataset.changeEmail]: false,
        currentDialog: "",
      },
      isMobile: isMobile || isTablet,
    };

    //Set unique contacts id
    const now = new Date().getTime();

    newState.profile.contacts.forEach((contact, index) => {
      contact.id = (now + index).toString();
    });

    return newState;
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

  toggleDialogsVisible = (e) => {
    const stateCopy = Object.assign({}, {}, this.state.dialogsVisible);
    const selectedDialog = e ? e.target.dataset.dialog : e;
    if (selectedDialog) {
      stateCopy[selectedDialog] = true;
      stateCopy.currentDialog = selectedDialog;
    } else {
      stateCopy[stateCopy.currentDialog] = false;
      stateCopy.currentDialog = "";
    }
    this.setState({ dialogsVisible: stateCopy });
  };

  onUserTypeChange(event) {
    var stateCopy = Object.assign({}, this.state);
    stateCopy.profile.isVisitor = event.target.value === "true";
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
    const { profile } = this.state;
    const errors = {
      firstName: !profile.firstName.trim(),
      lastName: !profile.lastName.trim(),
    };
    const hasError = errors.firstName || errors.lastName;

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
      .updateProfile(this.state.profile)
      .then((profile) => {
        this.props.updateProfileInUsers(profile);
        toastr.success(this.props.t("ChangesSavedSuccessfully"));
        setIsEditingForm(false);
        this.props.history.push(
          `${this.props.settings.homepage}/view/${profile.userName}`
        );
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

    if (document.referrer) {
      this.props.history.goBack();
    } else {
      setFilter(filter);
    }
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

  onLoadFileAvatar = (file, fileData) => {
    let data = new FormData();
    let _this = this;

    if (!file) {
      _this.onSaveAvatar(false);
      return;
    }

    data.append("file", file);
    data.append("Autosave", false);
    loadAvatar(this.state.profile.id, data)
      .then((response) => {
        var img = new Image();
        img.onload = function () {
          _this.setState({ isLoading: false });
          if (fileData) {
            fileData.avatar = {
              tmpFile: response.data,
              image: response.data,
              defaultWidth: img.width,
              defaultHeight: img.height,
            };
            if (!fileData.existImage) {
              _this.onSaveAvatar(fileData.existImage); // saving empty avatar
            } else {
              _this.onSaveAvatar(
                fileData.existImage,
                fileData.position,
                fileData.avatar
              );
            }
          }
        };
        img.src = response.data;
      })
      .catch((error) => {
        toastr.error(error);
        this.setState({ isLoading: false });
      });
  };

  onSaveAvatar = (isUpdate, result, avatar) => {
    this.setState({ isLoading: true });
    const { profile, setAvatarMax } = this.props;
    if (isUpdate) {
      createThumbnailsAvatar(profile.id, {
        x: Math.round(result.x * avatar.defaultWidth - result.width / 2),
        y: Math.round(result.y * avatar.defaultHeight - result.height / 2),
        width: result.width,
        height: result.height,
        tmpFile: avatar.tmpFile,
      })
        .then((response) => {
          let stateCopy = Object.assign({}, this.state);
          const avatarMax =
            response.max +
            "?_=" +
            Math.floor(Math.random() * Math.floor(10000));

          stateCopy.visibleAvatarEditor = false;
          stateCopy.isLoading = false;
          stateCopy.avatar.tmpFile = "";
          this.setState(stateCopy);

          setAvatarMax(avatarMax);
          this.setIsEdit();
          toastr.success(this.props.t("ChangesSavedSuccessfully"));
        })
        .catch((error) => {
          toastr.error(error);
          this.setState({ isLoading: false });
        })
        .then(() => {
          this.props.updateProfile(this.props.profile);
        })
        .then(() => {
          this.updateUserPhotoInState();
        })
        .then(() => this.props.fetchProfile(profile.id));
    } else {
      deleteAvatar(profile.id)
        .then((response) => {
          let stateCopy = Object.assign({}, this.state);
          stateCopy.visibleAvatarEditor = false;
          toastr.success(this.props.t("ChangesSavedSuccessfully"));
          this.setState(stateCopy);

          setAvatarMax(response.big);
          this.setIsEdit();
        })
        .catch((error) => toastr.error(error))
        .then(() => this.props.updateProfile(this.props.profile))
        .then(() => {
          this.setState(this.mapPropsToState(this.props));
        })
        .then(() => this.props.fetchProfile(profile.id));
    }
  };

  onCloseAvatarEditor() {
    this.setState({
      visibleAvatarEditor: false,
    });
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
    this.setIsEdit();
  }

  render() {
    const {
      isLoading,
      errors,
      profile,
      selector,
      dialogsVisible,
      isMobile,
    } = this.state;
    const { t, i18n, settings, avatarMax } = this.props;
    const {
      guestCaption,
      userCaption,
      regDateCaption,
      userPostCaption,
      groupCaption,
    } = settings.customNames;

    const pattern = getUserContactsPattern();
    const contacts = getUserContacts(profile.contacts);
    //TODO: inject guestsCaption in 'ProfileTypePopupHelper' key instead of hardcoded 'Guests'
    const tooltipTypeContent = (
      <>
        <Text style={{ paddingBottom: 17 }} fontSize="13px">
          {t("ProfileTypePopupHelper")}
        </Text>

        <Text fontSize="12px" as="div">
          <Table>
            <tbody>
              <tr>
                <Th>
                  <Text isBold fontSize="13px">
                    {t("ProductsAndInstruments_Products")}
                  </Text>
                </Th>
                <Th>
                  <Text isBold fontSize="13px">
                    {userCaption}
                  </Text>
                </Th>
                <Th>
                  <Text isBold fontSize="13px">
                    {guestCaption}
                  </Text>
                </Th>
              </tr>
              <tr>
                <Td>{t("Mail")}</Td>
                <Td>{t("ReviewingCustomMode")}</Td>
                <Td>-</Td>
              </tr>
              <tr>
                <Td>{t("DocumentsProduct")}</Td>
                <Td>{t("AccessRightsFullAccess")}</Td>
                <Td>{t("ViewAccess")}</Td>
              </tr>
              <tr>
                <Td>{t("ProjectsProduct")}</Td>
                <Td>{t("ReviewingCustomMode")}</Td>
                <Td>-</Td>
              </tr>
              <tr>
                <Td>{t("CommunityProduct")}</Td>
                <Td>{t("AccessRightsFullAccess")}</Td>
                <Td>{t("ViewAccess")}</Td>
              </tr>
              <tr>
                <Td>{t("People")}</Td>
                <Td>{t("ReviewingCustomMode")}</Td>
                <Td>-</Td>
              </tr>
              <tr>
                <Td>{t("Message")}</Td>
                <Td>{t("ReviewingCustomMode")}</Td>
                <Td>{t("ReviewingCustomMode")}</Td>
              </tr>
              <tr>
                <Td>{t("Calendar")}</Td>
                <Td>{t("ReviewingCustomMode")}</Td>
                <Td>{t("ReviewingCustomMode")}</Td>
              </tr>
            </tbody>
          </Table>
        </Text>
        <Link
          color="#316DAA"
          isHovered={true}
          href="https://helpcenter.onlyoffice.com/ru/gettingstarted/people.aspx#ManagingAccessRights_block"
          style={{ marginTop: 23 }}
        >
          {t("TermsOfUsePopupHelperLink")}
        </Link>
      </>
    );

    return (
      <>
        <MainContainer>
          <DataLossWarningDialog onContinue={this.onCancel} />
          <AvatarContainer>
            <Avatar
              size="max"
              role={getUserRole(profile)}
              source={this.props.avatarMax || profile.avatarMax}
              userName={profile.displayName}
              editing={true}
              editLabel={t("editAvatar")}
              editAction={
                isMobile ? this.openAvatarEditorPage : this.openAvatarEditor
              }
            />
            <AvatarEditor
              image={this.state.avatar.image}
              visible={this.state.visibleAvatarEditor}
              onClose={this.onCloseAvatarEditor}
              onSave={this.onSaveAvatar}
              onLoadFile={this.onLoadFileAvatar}
              headerLabel={t("EditPhoto")}
              selectNewPhotoLabel={t("selectNewPhotoLabel")}
              orDropFileHereLabel={t("orDropFileHereLabel")}
              unknownTypeError={t("ErrorUnknownFileImageType")}
              maxSizeFileError={t("maxSizeFileError")}
              unknownError={t("Error")}
              saveButtonLabel={
                this.state.isLoading ? t("UpdatingProcess") : t("SaveButton")
              }
              saveButtonLoading={this.state.isLoading}
            />
          </AvatarContainer>
          <MainFieldsContainer ref={this.mainFieldsContainerRef}>
            <TextChangeField
              labelText={`${t("Email")}:`}
              inputName="email"
              inputValue={profile.email}
              buttonText={t("ChangeButton")}
              buttonIsDisabled={isLoading}
              buttonOnClick={this.toggleDialogsVisible}
              buttonTabIndex={1}
              helpButtonHeaderContent={t("Mail")}
              tooltipContent={
                <Text fontSize="13px" as="div">
                  <Trans i18nKey="EmailPopupHelper" i18n={i18n}>
                    The main e-mail is needed to restore access to the portal in
                    case of loss of the password and send notifications.
                    <p
                      style={{
                        margin:
                          "1rem 0" /*, height: "0", visibility: "hidden"*/,
                      }}
                    >
                      You can create a new mail on the domain as the primary. In
                      this case, you must set a one-time password so that the
                      user can log in to the portal for the first time.
                    </p>
                    The main e-mail can be used as a login when logging in to
                    the portal.
                  </Trans>
                </Text>
              }
              dataDialog={dialogsDataset.changeEmail}
            />
            <TextChangeField
              labelText={`${t("Password")}:`}
              inputName="password"
              inputValue={profile.password}
              buttonText={t("ChangeButton")}
              buttonIsDisabled={isLoading}
              buttonOnClick={this.toggleDialogsVisible}
              buttonTabIndex={2}
              dataDialog={dialogsDataset.changePassword}
            />
            <TextChangeField
              labelText={`${t("Phone")}:`}
              inputName="phone"
              inputValue={profile.phone}
              buttonText={t("ChangeButton")}
              buttonIsDisabled={isLoading}
              buttonOnClick={this.toggleDialogsVisible}
              buttonTabIndex={3}
              dataDialog={dialogsDataset.changePhone}
            />
            <TextField
              isRequired={true}
              hasError={errors.firstName}
              labelText={`${t("FirstName")}:`}
              inputName="firstName"
              inputValue={profile.firstName}
              inputIsDisabled={isLoading}
              inputOnChange={this.onInputChange}
              inputAutoFocussed={true}
              inputTabIndex={4}
              maxLength={50}
            />
            <TextField
              isRequired={true}
              hasError={errors.lastName}
              labelText={`${t("LastName")}:`}
              inputName="lastName"
              inputValue={profile.lastName}
              inputIsDisabled={isLoading}
              inputOnChange={this.onInputChange}
              inputTabIndex={5}
              maxLength={50}
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
              inputTabIndex={6}
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
            <RadioField
              labelText={`${t("UserType")}:`}
              radioName="isVisitor"
              radioValue={profile.isVisitor.toString()}
              radioOptions={[
                { value: "true", label: guestCaption },
                { value: "false", label: userCaption },
              ]}
              radioIsDisabled={isLoading}
              radioOnChange={this.onUserTypeChange}
              tooltipContent={tooltipTypeContent}
              helpButtonHeaderContent={t("UserType")}
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
              inputTabIndex={7}
            />
            <TextField
              labelText={`${t("Location")}:`}
              inputName="location"
              inputValue={profile.location}
              inputIsDisabled={isLoading}
              inputOnChange={this.onInputChange}
              inputTabIndex={8}
            />
            <TextField
              labelText={`${userPostCaption}:`}
              inputName="title"
              inputValue={profile.title}
              inputIsDisabled={isLoading}
              inputOnChange={this.onInputChange}
              inputTabIndex={9}
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
            tabIndex={10}
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
            tabIndex={11}
          />
          <Button
            label={t("CancelButton")}
            onClick={this.onCancelHandler}
            isDisabled={isLoading}
            size="big"
            style={{ marginLeft: "8px" }}
            tabIndex={12}
          />
        </div>

        {dialogsVisible.changeEmail && (
          <ChangeEmailDialog
            visible={dialogsVisible.changeEmail}
            onClose={this.toggleDialogsVisible}
            user={profile}
          />
        )}

        {dialogsVisible.changePassword && (
          <ChangePasswordDialog
            visible={dialogsVisible.changePassword}
            onClose={this.toggleDialogsVisible}
            email={profile.email}
          />
        )}

        {dialogsVisible.changePhone && (
          <ChangePhoneDialog
            visible={dialogsVisible.changePhone}
            onClose={this.toggleDialogsVisible}
            user={profile}
          />
        )}
      </>
    );
  }
}

const mapStateToProps = (state) => {
  return {
    profile: state.profile.targetUser,
    avatarMax: state.profile.avatarMax,
    settings: state.auth.settings,
    groups: state.people.groups,
    editingForm: state.people.editingForm,
    filter: state.people.filter,
  };
};

export default connect(mapStateToProps, {
  updateProfile,
  fetchProfile,
  updateProfileInUsers,
  setIsVisibleDataLossDialog,
  setIsEditingForm,
  setFilter,
  toggleAvatarEditor,
  setAvatarMax,
})(withRouter(withTranslation()(UpdateUserForm)));
