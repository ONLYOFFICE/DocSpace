import React from 'react'
import { withRouter } from 'react-router'
import { connect } from 'react-redux'
import { Avatar, Button, Textarea, Text, toastr, AvatarEditor, Link } from 'asc-web-components'
import { withTranslation, Trans } from 'react-i18next';
import { toEmployeeWrapper, getUserRole, getUserContactsPattern, getUserContacts, mapGroupsToGroupSelectorOptions, mapGroupSelectorOptionsToGroups, filterGroupSelectorOptions } from "../../../../../store/people/selectors";
import { updateProfile, getUserPhoto } from '../../../../../store/profile/actions'
import { MainContainer, AvatarContainer, MainFieldsContainer } from './FormFields/Form'
import TextField from './FormFields/TextField'
import TextChangeField from './FormFields/TextChangeField'
import DateField from './FormFields/DateField'
import RadioField from './FormFields/RadioField'
import DepartmentField from './FormFields/DepartmentField'
import ContactsField from './FormFields/ContactsField'
import InfoFieldContainer from './FormFields/InfoFieldContainer'
import { departments, department, position, employedSinceDate, typeGuest, typeUser } from '../../../../../helpers/customNames';
import styled from "styled-components";
import { api } from "asc-web-common";
import { ChangeEmailDialog, ChangePasswordDialog, ChangePhoneDialog } from '../../../../dialogs';
const { createThumbnailsAvatar, loadAvatar, deleteAvatar } = api.people;

const dialogsDataset = {
  changeEmail: 'changeEmail',
  changePassword: 'changePassword',
  changePhone: 'changePhone'
};

const Table = styled.table`
  width: 100%;
  margin-bottom: 23px;
`;

const Th = styled.th`
  padding: 11px 0 10px 0px;
  border-top: 1px solid #ECEEF1;
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

    this.onContactsItemAdd = this.onContactsItemAdd.bind(this);
    this.onContactsItemTypeChange = this.onContactsItemTypeChange.bind(this);
    this.onContactsItemTextChange = this.onContactsItemTextChange.bind(this);
    this.onContactsItemRemove = this.onContactsItemRemove.bind(this);

    this.openAvatarEditor = this.openAvatarEditor.bind(this);
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
  }

  mapPropsToState = (props) => {
    var profile = toEmployeeWrapper(props.profile);
    var allOptions = mapGroupsToGroupSelectorOptions(props.groups);
    var selected = mapGroupsToGroupSelectorOptions(profile.groups);

    getUserPhoto(profile.id).then(userPhotoData => {
      if (userPhotoData.original) {
        let avatarDefaultSizes = /_(\d*)-(\d*)./g.exec(userPhotoData.original);
        if (avatarDefaultSizes !== null && avatarDefaultSizes.length > 2) {
          this.setState({
            avatar: {
              tmpFile: this.state.avatar.tmpFile,
              defaultWidth: avatarDefaultSizes[1],
              defaultHeight: avatarDefaultSizes[2],
              image: userPhotoData.original ? userPhotoData.original.indexOf('default_user_photo') !== -1 ? null : userPhotoData.original : null
            }
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
        selected: selected
      },
      avatar: {
        tmpFile: "",
        image: null,
        defaultWidth: 0,
        defaultHeight: 0
      },
      dialogsVisible: {
        [dialogsDataset.changePassword]: false,
        [dialogsDataset.changePhone]: false,
        [dialogsDataset.changeEmail]: false,
        currentDialog: ''
      }
    };

    //Set unique contacts id 
    const now = new Date().getTime();

    newState.profile.contacts.forEach((contact, index) => {
      contact.id = (now + index).toString();
    });

    return newState;
  }

  onInputChange(event) {
    var stateCopy = Object.assign({}, this.state);
    stateCopy.profile[event.target.name] = event.target.value;
    this.setState(stateCopy)
  }

  toggleDialogsVisible = (e) => {
    const stateCopy = Object.assign({}, {}, this.state.dialogsVisible);
    const selectedDialog = e ? e.target.dataset.dialog : e;
    if (selectedDialog) {
      stateCopy[selectedDialog] = true;
      stateCopy.currentDialog = selectedDialog;
    }
    else {
      stateCopy[stateCopy.currentDialog] = false;
      stateCopy.currentDialog = '';
    }
    this.setState({ dialogsVisible: stateCopy });
  };

  onUserTypeChange(event) {
    var stateCopy = Object.assign({}, this.state);
    stateCopy.profile.isVisitor = event.target.value === "true";
    this.setState(stateCopy)
  }

  onBirthdayDateChange(value) {
    var stateCopy = Object.assign({}, this.state);
    stateCopy.profile.birthday = value ? value.toJSON() : null;
    this.setState(stateCopy)
  }

  onWorkFromDateChange(value) {
    var stateCopy = Object.assign({}, this.state);
    stateCopy.profile.workFrom = value ? value.toJSON() : null;
    this.setState(stateCopy)
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
    if (!this.validate())
      return false;

    this.setState({ isLoading: true });

    this.props.updateProfile(this.state.profile)
      .then((profile) => {
        toastr.success(this.props.t("ChangesSavedSuccessfully"));
        this.props.history.push(`${this.props.settings.homepage}/view/${profile.userName}`);
      })
      .catch((error) => {
        toastr.error(error);
        this.setState({ isLoading: false });
      });
  }

  onCancel() {
    this.props.history.goBack();
  }

  onContactsItemAdd(item) {
    var stateCopy = Object.assign({}, this.state);
    stateCopy.profile.contacts.push({
      id: new Date().getTime().toString(),
      type: item.value,
      value: ""
    });
    this.setState(stateCopy);
  }

  onContactsItemTypeChange(item) {
    const id = item.key.split("_")[0];
    var stateCopy = Object.assign({}, this.state);
    stateCopy.profile.contacts.forEach(element => {
      if (element.id === id)
        element.type = item.value;
    });
    this.setState(stateCopy);
  }

  onContactsItemTextChange(event) {
    const id = event.target.name.split("_")[0];
    var stateCopy = Object.assign({}, this.state);
    stateCopy.profile.contacts.forEach(element => {
      if (element.id === id)
        element.value = event.target.value;
    });
    this.setState(stateCopy);
  }

  onContactsItemRemove(event) {
    const id = event.target.closest(".remove_icon").dataset.for.split("_")[0];
    var stateCopy = Object.assign({}, this.state);
    const filteredArray = stateCopy.profile.contacts.filter((element) => { 
      return element.id !== id;
    });
    stateCopy.profile.contacts = filteredArray;
    this.setState(stateCopy);
  }

  openAvatarEditor(){
    let avatarDefault = this.state.avatar.image;
    let avatarDefaultSizes = /_orig_(\d*)-(\d*)./g.exec(this.state.avatar.image);
    if (avatarDefault !== null && avatarDefaultSizes !== null && avatarDefaultSizes.length > 2) {
      this.setState({
        avatar: {
          tmpFile: this.state.avatar.tmpFile,
          image: this.state.avatar.image,
          defaultWidth: avatarDefaultSizes[1],
          defaultHeight: avatarDefaultSizes[2]
        }
      })
    }
    this.setState({
      visibleAvatarEditor: true,
    });
  }

  onLoadFileAvatar(file) {
    let data = new FormData();
    let _this = this;
    data.append("file", file);
    data.append("Autosave", false);
    loadAvatar(this.state.profile.id, data)
      .then((response) => {
        var img = new Image();
        img.onload = function () {
          var stateCopy = Object.assign({}, _this.state);
          stateCopy.avatar = {
            tmpFile: response.data,
            image: response.data,
            defaultWidth: img.width,
            defaultHeight: img.height
          }
          _this.setState(stateCopy);
        };
        img.src = response.data;
      })
      .catch((error) => toastr.error(error));
  }

  onSaveAvatar(isUpdate, result) {
    if (isUpdate) {
      createThumbnailsAvatar(this.state.profile.id, {
        x: Math.round(result.x * this.state.avatar.defaultWidth - result.width / 2),
        y: Math.round(result.y * this.state.avatar.defaultHeight - result.height / 2),
        width: result.width,
        height: result.height,
        tmpFile: this.state.avatar.tmpFile
      })
        .then((response) => {
          let stateCopy = Object.assign({}, this.state);
          stateCopy.visibleAvatarEditor = false;
          stateCopy.avatar.tmpFile = '';
          stateCopy.profile.avatarMax = response.max + '?_=' + Math.floor(Math.random() * Math.floor(10000));
          toastr.success(this.props.t("ChangesSavedSuccessfully"));
          this.setState(stateCopy);
        })
        .catch((error) => toastr.error(error));
    } else {
      deleteAvatar(this.state.profile.id)
        .then((response) => {
          let stateCopy = Object.assign({}, this.state);
          stateCopy.visibleAvatarEditor = false;
          stateCopy.profile.avatarMax = response.big;
          toastr.success(this.props.t("ChangesSavedSuccessfully"));
          this.setState(stateCopy);
        })
        .catch((error) => toastr.error(error));
    }
  }

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
    stateCopy.selector.options = filterGroupSelectorOptions(stateCopy.selector.allOptions, template);
    this.setState(stateCopy);
  }

  onSelectGroups(selected) {
    var stateCopy = Object.assign({}, this.state);
    stateCopy.profile.groups = mapGroupSelectorOptionsToGroups(selected);
    stateCopy.selector.selected = selected;
    stateCopy.selector.visible = false;
    this.setState(stateCopy);
  }

  onRemoveGroup(id) {
    var stateCopy = Object.assign({}, this.state);
    stateCopy.profile.groups = stateCopy.profile.groups.filter(group => group.id !== id);
    stateCopy.selector.selected = stateCopy.selector.selected.filter(option => option.key !== id);
    this.setState(stateCopy)
  }

  render() {
    const { isLoading, errors, profile, selector, dialogsVisible } = this.state;
    const { t, i18n } = this.props;

    const pattern = getUserContactsPattern();
    const contacts = getUserContacts(profile.contacts);
    const tooltipTypeContent =
      <>
        <Text
          style={{ paddingBottom: 17 }}
          fontSize='13px'>
          {t("ProfileTypePopupHelper")}
        </Text>

        <Text fontSize='12px' as="div">
          <Table>
            <tbody>
              <tr>
                <Th>
                  <Text isBold fontSize='13px'>
                    {t("ProductsAndInstruments_Products")}
                  </Text>
                </Th>
                <Th>
                  <Text isBold fontSize='13px'>
                    {t("Employee")}
                  </Text>
                </Th>
                <Th>
                  <Text isBold fontSize='13px'>
                    {t("GuestCaption")}
                  </Text>
                </Th>
              </tr>
              <tr>
                <Td>{t("Mail")}</Td>
                <Td>review</Td>
                <Td>-</Td>
              </tr>
              <tr>
                <Td>{t("DocumentsProduct")}</Td>
                <Td>full access</Td>
                <Td>view</Td>
              </tr>
              <tr>
                <Td>{t("ProjectsProduct")}</Td>
                <Td>review</Td>
                <Td>-</Td>
              </tr>
              <tr>
                <Td>{t("CommunityProduct")}</Td>
                <Td>full access</Td>
                <Td>view</Td>
              </tr>
              <tr>
                <Td>{t("People")}</Td>
                <Td>review</Td>
                <Td>-</Td>
              </tr>
              <tr>
                <Td>{t("Message")}</Td>
                <Td>review</Td>
                <Td>review</Td>
              </tr>
              <tr>
                <Td>{t("Calendar")}</Td>
                <Td>review</Td>
                <Td>review</Td>
              </tr>
            </tbody>
          </Table>
        </Text>
        <Link
          color="#316DAA"
          isHovered={true}
          href="https://helpcenter.onlyoffice.com/ru/gettingstarted/people.aspx#ManagingAccessRights_block"
          style={{ marginTop: 23 }}>
          {t("TermsOfUsePopupHelperLink")}
        </Link>
      </>;

    return (
      <>
        <MainContainer>
          <AvatarContainer>
            <Avatar
              size="max"
              role={getUserRole(profile)}
              source={profile.avatarMax}
              userName={profile.displayName}
              editing={true}
              editLabel={t("EditPhoto")}
              editAction={this.openAvatarEditor}
            />
            <AvatarEditor
              image={this.state.avatar.image}
              visible={this.state.visibleAvatarEditor}
              onClose={this.onCloseAvatarEditor}
              onSave={this.onSaveAvatar}
              onLoadFile={this.onLoadFileAvatar}
              headerLabel={t("editAvatar")}
              chooseFileLabel={t("chooseFileLabel")}
              chooseMobileFileLabel={t("chooseMobileFileLabel")}
              unknownTypeError={t("unknownTypeError")}
              maxSizeFileError={t("maxSizeFileError")}
              unknownError={t("unknownError")}
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
                <Text fontSize='13px' as="div">
                  <Trans i18nKey="EmailPopupHelper" i18n={i18n}>
                    The main e-mail is needed to restore access to the portal in case of loss of the password and send notifications.
                    <p style={{ margin: "1rem 0"/*, height: "0", visibility: "hidden"*/ }}>
                      You can create a new mail on the domain as the primary.
                      In this case, you must set a one-time password so that the user can log in to the portal for the first time.
                    </p>
                    The main e-mail can be used as a login when logging in to the portal.
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
            />
            <DateField
              calendarHeaderContent={t("CalendarSelectDate")}
              labelText={`${t("Birthdate")}:`}
              inputName="birthday"
              inputValue={profile.birthday ? new Date(profile.birthday) : undefined}
              inputIsDisabled={isLoading}
              inputOnChange={this.onBirthdayDateChange}
              inputTabIndex={6}
            />
            <RadioField
              labelText={`${t("Sex")}:`}
              radioName="sex"
              radioValue={profile.sex}
              radioOptions={[
                { value: 'male', label: t("SexMale") },
                { value: 'female', label: t("SexFemale") }
              ]}
              radioIsDisabled={isLoading}
              radioOnChange={this.onInputChange}
            />
            <RadioField
              labelText={`${t("UserType")}:`}
              radioName="isVisitor"
              radioValue={profile.isVisitor.toString()}
              radioOptions={[
                { value: "true", label: t("CustomTypeGuest", { typeGuest }) },
                { value: "false", label: t("CustomTypeUser", { typeUser }) }
              ]}
              radioIsDisabled={isLoading}
              radioOnChange={this.onUserTypeChange}

              tooltipContent={tooltipTypeContent}
              helpButtonHeaderContent={t('UserType')}
            />
            <DateField
              calendarHeaderContent={t("CalendarSelectDate")}
              labelText={`${t("CustomEmployedSinceDate", { employedSinceDate })}:`}
              inputName="workFrom"
              inputValue={profile.workFrom ? new Date(profile.workFrom) : undefined}
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
              labelText={`${t("CustomPosition", { position })}:`}
              inputName="title"
              inputValue={profile.title}
              inputIsDisabled={isLoading}
              inputOnChange={this.onInputChange}
              inputTabIndex={9}
            />
            <DepartmentField
              labelText={`${t("CustomDepartment", { department })}:`}
              isDisabled={isLoading}
              showGroupSelectorButtonTitle={t("AddButton")}
              onShowGroupSelector={this.onShowGroupSelector}
              onCloseGroupSelector={this.onCloseGroupSelector}
              onRemoveGroup={this.onRemoveGroup}
              selectorIsVisible={selector.visible}
              searchPlaceHolderLabel={t("SearchDepartments")}
              selectorOptions={selector.options}
              selectorSelectedOptions={selector.selected}
              selectorAddButtonText={t("CustomAddDepartments", { departments })}
              selectorSelectAllText={t("SelectAll")}
              selectorOnSearchGroups={this.onSearchGroups}
              selectorOnSelectGroups={this.onSelectGroups}
            />
          </MainFieldsContainer>
        </MainContainer>
        <InfoFieldContainer headerText={t("Comments")}>
          <Textarea placeholder={t("AddСomment")} name="notes" value={profile.notes} isDisabled={isLoading} onChange={this.onInputChange} tabIndex={10}/> 
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
          <Button label={t("SaveButton")} onClick={this.handleSubmit} primary isDisabled={isLoading} size="big" tabIndex={11} />
          <Button label={t("CancelButton")} onClick={this.onCancel} isDisabled={isLoading} size="big" style={{ marginLeft: "8px" }} tabIndex={12} />
        </div>

        {dialogsVisible.changeEmail &&
          <ChangeEmailDialog
            visible={dialogsVisible.changeEmail}
            onClose={this.toggleDialogsVisible}
            user={profile}
          />
        }

        {dialogsVisible.changePassword &&
          <ChangePasswordDialog
            visible={dialogsVisible.changePassword}
            onClose={this.toggleDialogsVisible}
            email={profile.email}
          />
        }

        {dialogsVisible.changePhone &&
          <ChangePhoneDialog
            visible={dialogsVisible.changePhone}
            onClose={this.toggleDialogsVisible}
            user={profile}
          />
        }
      </>
    );
  };
}

const mapStateToProps = (state) => {
  return {
    profile: state.profile.targetUser,
    settings: state.auth.settings,
    groups: state.people.groups
  }
};

export default connect(
  mapStateToProps,
  {
    updateProfile
  }
)(withRouter(withTranslation()(UpdateUserForm)));