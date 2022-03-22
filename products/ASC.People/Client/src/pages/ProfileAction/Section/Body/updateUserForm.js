import React from "react";
import { withRouter } from "react-router";

import Avatar from "@appserver/components/avatar";
import Button from "@appserver/components/button";
import Textarea from "@appserver/components/textarea";
import Text from "@appserver/components/text";
import AvatarEditor from "@appserver/components/avatar-editor";
import Link from "@appserver/components/link";
import { isTablet } from "@appserver/components/utils/device";

import { withTranslation, Trans } from "react-i18next";

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
import { DataLossWarningDialog } from "../../../../components/dialogs";
import {
  createThumbnailsAvatar,
  loadAvatar,
  deleteAvatar,
} from "@appserver/common/api/people";
import toastr from "studio/toastr";
import {
  ChangeEmailDialog,
  ChangePasswordDialog,
  ChangePhoneDialog,
} from "../../../../components/dialogs";
import { isMobile } from "react-device-detect";
import { inject, observer } from "mobx-react";
import {
  filterGroupSelectorOptions,
  getUserContacts,
  getUserContactsPattern,
  getUserRole,
  mapGroupSelectorOptionsToGroups,
  mapGroupsToGroupSelectorOptions,
  toEmployeeWrapper,
} from "../../../../helpers/people-helpers";
import config from "../../../../../package.json";
import { combineUrl } from "@appserver/common/utils";
import { AppServerConfig } from "@appserver/common/constants";

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

    this.mainFieldsContainerRef = React.createRef();
  }

  componentDidMount() {
    this.props.setIsEditTargetUser(true);

    this.unblock = this.props.history.block((targetLocation) => {
      if (this.props.isEdit) {
        this.props.setIsVisibleDataLossDialog(true);
        return false;
      }

      return true;
    });

    window.addEventListener("beforeunload", this.handleWindowBeforeUnload);
  }

  componentWillUnmount() {
    this.unblock();
    window.removeEventListener("beforeunload", this.handleWindowBeforeUnload);
  }

  handleWindowBeforeUnload = (e) => {
    if (this.props.isEdit) {
      e.preventDefault();
      e.returnValue = "";
    }
  };

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
    this.props.getUserPhoto(profile.id).then((userPhotoData) => {
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

    this.props.getUserPhoto(profile.id).then((userPhotoData) => {
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

  setIsEdit = () => {
    const { isEdit, setIsEditingForm } = this.props;
    if (!isEdit) setIsEditingForm(true);
  };

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

  onUserTypeChange = (event) => {
    var stateCopy = Object.assign({}, this.state);
    stateCopy.profile.isVisitor = event.target.value === "true";
    this.setState(stateCopy);
    this.setIsEdit();
  };

  onBirthdayDateChange = (value) => {
    var stateCopy = Object.assign({}, this.state);
    const birthday = value ? value.toJSON() : stateCopy.profile.workFrom;
    stateCopy.profile.birthday = birthday;

    if (new Date(birthday) > new Date(stateCopy.profile.workFrom)) {
      stateCopy.profile.workFrom = birthday;
    }

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
    const { profile, errors } = this.state;

    if (errors.firstName || errors.lastName) {
      this.scrollToErrorForm();
      return;
    }

    const errorsObj = {
      firstName: !profile.firstName.trim(),
      lastName: !profile.lastName.trim(),
    };
    const hasError = errorsObj.firstName || errorsObj.lastName;

    if (hasError) {
      this.scrollToErrorForm();
    }

    this.setState({ errors: errorsObj });
    return !hasError;
  };

  handleSubmit = () => {
    if (!this.validate()) return false;

    const {
      setIsEditingForm,
      updateProfile,
      updateProfileInUsers,
      history,
      t,
      setUserIsUpdate,
    } = this.props;

    this.setState({ isLoading: true });

    updateProfile(this.state.profile)
      .then((profile) => {
        updateProfileInUsers(profile);
        toastr.success(t("ChangesSavedSuccessfully"));
        setIsEditingForm(false);
        setUserIsUpdate(true);
        history.goBack();
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
    const {
      filter,
      setFilter,
      history,
      isEditTargetUser,
      profile,
      personal,
    } = this.props;

    this.unblock();

    if (personal) {
      history.push(combineUrl(AppServerConfig.proxyURL, "/my"));
    } else if (isEditTargetUser || document.referrer) {
      history.push(
        combineUrl(
          AppServerConfig.proxyURL,
          config.homepage,
          `/view/${profile.userName}`
        )
      );
    } else {
      history.push(combineUrl(AppServerConfig.proxyURL, config.homepage));
      setFilter(filter);
    }
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

    if (!file) {
      _this.onSaveAvatar(false);
      return;
    }

    data.append("file", file);
    data.append("Autosave", false);
    loadAvatar(this.state.profile.id, data)
      .then((response) => {
        if (!response.success && response.message) {
          throw response.message;
        }
        var img = new Image();
        img.onload = () => {
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
    const { profile, setAvatarMax, personal } = this.props;

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
        .then(() => !personal && this.props.fetchProfile(profile.id));
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
        .then(() => !personal && this.props.fetchProfile(profile.id));
    }
  };

  onCloseAvatarEditor = () => this.setState({ visibleAvatarEditor: false });

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
    this.setIsEdit();
  };

  onSaveClick = () => this.setState({ isLoading: true });

  render() {
    const {
      isLoading,
      errors,
      profile,
      selector,
      dialogsVisible,
      isMobile,
    } = this.state;
    const {
      t,
      customNames,
      //avatarMax,
      disableProfileType,
      isAdmin,
      isMy,
      isSelf,
      language,
      personal,
      isTabletView,
    } = this.props;
    const {
      guestCaption,
      userCaption,
      regDateCaption,
      userPostCaption,
      groupCaption,
    } = customNames;

    const maxLabelWidth = "140px";

    const pattern = getUserContactsPattern();
    const contacts = getUserContacts(profile.contacts);
    const notEmptyFirstName = Boolean(profile.firstName.trim());
    const notEmptyLastName = Boolean(profile.lastName.trim());
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
                <Td>{t("Common:Mail")}</Td>
                <Td>{t("Common:Review")}</Td>
                <Td>-</Td>
              </tr>
              <tr>
                <Td>{t("Common:Documents")}</Td>
                <Td>{t("Common:FullAccess")}</Td>
                <Td>{t("Common:View")}</Td>
              </tr>
              <tr>
                <Td>{t("Common:ProjectsProduct")}</Td>
                <Td>{t("Common:Review")}</Td>
                <Td>-</Td>
              </tr>
              <tr>
                <Td>{t("Common:CommunityProduct")}</Td>
                <Td>{t("Common:FullAccess")}</Td>
                <Td>{t("Common:View")}</Td>
              </tr>
              <tr>
                <Td>{t("Common:People")}</Td>
                <Td>{t("Common:Review")}</Td>
                <Td>-</Td>
              </tr>
              <tr>
                <Td>{t("Message")}</Td>
                <Td>{t("Common:Review")}</Td>
                <Td>{t("Common:Review")}</Td>
              </tr>
              <tr>
                <Td>{t("Calendar")}</Td>
                <Td>{t("Common:Review")}</Td>
                <Td>{t("Common:Review")}</Td>
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

    const radioIsDisabled =
      isSelf || (profile.listAdminModules && !!profile.listAdminModules.length);

    const calendarWorkFrom = profile.workFrom
      ? new Date(profile.workFrom)
      : undefined;

    const calendarMinDate = profile.birthday
      ? new Date(profile.birthday)
      : calendarWorkFrom;

    const birthdayDateValue = profile.birthday
      ? new Date(profile.birthday)
      : new Date(this.props.profile.workFrom);

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
              editAction={
                isMobile ? this.openAvatarEditorPage : this.openAvatarEditor
              }
            />
            <AvatarEditor
              image={this.state.avatar.image}
              visible={this.state.visibleAvatarEditor}
              onClose={this.onCloseAvatarEditor}
              onSave={this.onSaveClick}
              onLoadFile={this.onLoadFileAvatar}
              headerLabel={t("EditPhoto")}
              selectNewPhotoLabel={t("Translations:selectNewPhotoLabel")}
              orDropFileHereLabel={t("Translations:orDropFileHereLabel")}
              unknownTypeError={t("Translations:ErrorUnknownFileImageType")}
              maxSizeFileError={t("Translations:maxSizeFileError")}
              unknownError={t("Common:Error")}
              saveButtonLabel={
                isLoading ? t("UpdatingProcess") : t("Common:SaveButton")
              }
              saveButtonLoading={isLoading}
              maxSizeLabel={t("Translations:MaxSizeLabel")}
            />
          </AvatarContainer>
          <MainFieldsContainer
            ref={this.mainFieldsContainerRef}
            noSelect
            {...(!isTabletView && { marginBottom: "32px" })}
          >
            <TextChangeField
              labelText={`${t("Common:Email")}:`}
              inputName="email"
              inputValue={profile.email}
              buttonText={t("ChangeButton")}
              buttonIsDisabled={isLoading}
              buttonOnClick={this.toggleDialogsVisible}
              buttonTabIndex={1}
              helpButtonHeaderContent={t("Common:Mail")}
              tooltipContent={
                <Text fontSize="13px" as="div">
                  <Trans t={t} i18nKey="EmailPopupHelper" ns="ProfileAction">
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
              maxLabelWidth={maxLabelWidth}
            />
            <TextChangeField
              labelText={`${t("Common:Password")}:`}
              inputName="password"
              inputValue={"********"}
              buttonText={t("ChangeButton")}
              buttonIsDisabled={isLoading}
              buttonOnClick={this.toggleDialogsVisible}
              buttonTabIndex={2}
              dataDialog={dialogsDataset.changePassword}
              maxLabelWidth={maxLabelWidth}
            />
            {/*TODO: uncomment this after added phone form */}
            {/* <TextChangeField
              labelText={`${t("Common:Phone")}:`}
              inputName="phone"
              inputValue={profile.mobilePhone}
              buttonText={t("ChangeButton")}
              buttonIsDisabled={isLoading}
              buttonOnClick={this.toggleDialogsVisible}
              buttonTabIndex={3}
              dataDialog={dialogsDataset.changePhone}
              maxLabelWidth={maxLabelWidth}
            /> */}
            <TextField
              isRequired={true}
              hasError={errors.firstName}
              labelText={`${t("FirstName")}:`}
              {...(notEmptyFirstName && {
                errorMessage: t("ErrorInvalidUserFirstName"),
              })}
              inputName="firstName"
              inputValue={profile.firstName}
              inputIsDisabled={isLoading}
              inputOnChange={this.onInputChange}
              inputAutoFocussed={!isMobile}
              inputTabIndex={4}
              maxLength={50}
              maxLabelWidth={maxLabelWidth}
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
              inputTabIndex={5}
              maxLength={50}
              maxLabelWidth={maxLabelWidth}
            />
            <DateField
              calendarHeaderContent={`${t("CalendarSelectDate")}:`}
              labelText={`${t("Translations:Birthdate")}:`}
              inputName="birthday"
              inputClassName="date-picker_input-birthday"
              inputValue={birthdayDateValue}
              inputIsDisabled={isLoading}
              inputOnChange={this.onBirthdayDateChange}
              inputTabIndex={6}
              locale={language}
              maxLabelWidth={maxLabelWidth}
            />
            <RadioField
              labelText={`${t("Translations:Sex")}:`}
              radioName="sex"
              radioValue={profile.sex}
              radioOptions={[
                { value: "male", label: t("Translations:MaleSexStatus") },
                { value: "female", label: t("Translations:FemaleSexStatus") },
              ]}
              radioIsDisabled={isLoading}
              radioOnChange={this.onInputChange}
              maxLabelWidth={maxLabelWidth}
            />
            {!personal && (
              <RadioField
                labelText={`${t("Common:Type")}:`}
                radioName="isVisitor"
                radioValue={profile.isVisitor.toString()}
                radioOptions={[
                  { value: "true", label: guestCaption },
                  { value: "false", label: userCaption },
                ]}
                radioIsDisabled={
                  isLoading || disableProfileType || radioIsDisabled || isMy
                }
                radioOnChange={this.onUserTypeChange}
                tooltipContent={tooltipTypeContent}
                helpButtonHeaderContent={t("Common:Type")}
                maxLabelWidth={maxLabelWidth}
              />
            )}
            {!personal && (
              <DateField
                calendarHeaderContent={`${t("CalendarSelectDate")}:`}
                labelText={`${regDateCaption}:`}
                inputName="workFrom"
                inputClassName="date-picker_input-reg-date"
                inputValue={calendarWorkFrom}
                inputIsDisabled={
                  isLoading ||
                  !isAdmin ||
                  calendarMinDate >= new Date().setHours(0, 0, 0, 0)
                }
                inputOnChange={this.onWorkFromDateChange}
                inputTabIndex={7}
                calendarMinDate={calendarMinDate}
                locale={language}
                maxLabelWidth={maxLabelWidth}
                //calendarMaxDate={calendarMinDate}
              />
            )}
            <TextField
              labelText={`${t("Translations:Location")}:`}
              inputName="location"
              inputValue={profile.location}
              inputIsDisabled={isLoading}
              inputOnChange={this.onInputChange}
              inputTabIndex={8}
              maxLabelWidth={maxLabelWidth}
            />
            {!personal && (
              <TextField
                labelText={`${userPostCaption}:`}
                inputName="title"
                inputValue={profile.title}
                inputIsDisabled={isLoading || !isAdmin}
                inputOnChange={this.onInputChange}
                inputTabIndex={9}
                maxLabelWidth={maxLabelWidth}
              />
            )}
            {!isMy && !personal && (
              <DepartmentField
                labelText={`${groupCaption}:`}
                isDisabled={isLoading || !isAdmin}
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
                maxLabelWidth={maxLabelWidth}
              />
            )}
          </MainFieldsContainer>
        </MainContainer>
        {!personal && (
          <InfoFieldContainer
            headerText={t("Translations:Comments")}
            marginBottom={"42px"}
          >
            <Textarea
              placeholder={t("WriteComment")}
              name="notes"
              value={profile.notes}
              isDisabled={isLoading}
              onChange={this.onInputChange}
              tabIndex={10}
            />
          </InfoFieldContainer>
        )}
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
          headerText={t("Translations:SocialProfiles")}
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
            size="big"
            tabIndex={11}
          />
          <Button
            label={t("Common:CancelButton")}
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

export default withRouter(
  inject(({ auth, peopleStore }) => ({
    customNames: auth.settingsStore.customNames,
    isAdmin: auth.isAdmin,
    language: auth.language,
    groups: peopleStore.groupsStore.groups,
    isEdit: peopleStore.editingFormStore.isEdit,
    setIsVisibleDataLossDialog:
      peopleStore.editingFormStore.setIsVisibleDataLossDialog,
    setIsEditingForm: peopleStore.editingFormStore.setIsEditingForm,
    filter: peopleStore.filterStore.filter,
    setFilter: peopleStore.filterStore.setFilterParams,
    toggleAvatarEditor: peopleStore.avatarEditorStore.toggleAvatarEditor,
    profile: peopleStore.targetUserStore.targetUser,
    fetchProfile: peopleStore.targetUserStore.getTargetUser,
    avatarMax: peopleStore.avatarEditorStore.avatarMax,
    setAvatarMax: peopleStore.avatarEditorStore.setAvatarMax,
    updateProfileInUsers: peopleStore.usersStore.updateProfileInUsers,
    updateProfile: peopleStore.targetUserStore.updateProfile,
    getUserPhoto: peopleStore.targetUserStore.getUserPhoto,
    disableProfileType: peopleStore.targetUserStore.getDisableProfileType,
    isSelf: peopleStore.targetUserStore.isMe,
    setIsEditTargetUser: peopleStore.targetUserStore.setIsEditTargetUser,
    isEditTargetUser: peopleStore.targetUserStore.isEditTargetUser,
    personal: auth.settingsStore.personal,
    setUserIsUpdate: auth.userStore.setUserIsUpdate,
    userFormValidation: auth.settingsStore.userFormValidation,
    isTabletView: auth.settingsStore.isTabletView,
  }))(
    observer(
      withTranslation(["ProfileAction", "Common", "Translations"])(
        UpdateUserForm
      )
    )
  )
);
