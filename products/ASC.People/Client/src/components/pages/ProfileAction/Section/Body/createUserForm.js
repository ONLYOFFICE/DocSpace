import React from 'react'
import { withRouter } from 'react-router'
import { connect } from 'react-redux'
import { Avatar, Button, Textarea, toastr, AdvancedSelector } from 'asc-web-components'
import { withTranslation } from 'react-i18next';
import { toEmployeeWrapper, getUserRole, getUserContactsPattern, getUserContacts } from "../../../../../store/people/selectors";
import { createProfile } from '../../../../../store/profile/actions';
import { MainContainer, AvatarContainer, MainFieldsContainer } from './FormFields/Form'
import TextField from './FormFields/TextField'
import PasswordField from './FormFields/PasswordField'
import DateField from './FormFields/DateField'
import RadioField from './FormFields/RadioField'
import DepartmentField from './FormFields/DepartmentField'
import ContactsField from './FormFields/ContactsField'
import InfoFieldContainer from './FormFields/InfoFieldContainer'
import { department, position, employedSinceDate } from '../../../../../helpers/customNames';

class CreateUserForm extends React.Component {

  constructor(props) {
    super(props);

    this.state = this.mapPropsToState(props);

    this.validate = this.validate.bind(this);
    this.handleSubmit = this.handleSubmit.bind(this);
    this.onInputChange = this.onInputChange.bind(this);
    this.onBirthdayDateChange = this.onBirthdayDateChange.bind(this);
    this.onWorkFromDateChange = this.onWorkFromDateChange.bind(this);
    this.onGroupClose = this.onGroupClose.bind(this);
    this.onCancel = this.onCancel.bind(this);

    this.onContactsItemAdd = this.onContactsItemAdd.bind(this);
    this.onContactsItemTypeChange = this.onContactsItemTypeChange.bind(this);
    this.onContactsItemTextChange = this.onContactsItemTextChange.bind(this);

    this.onAddGroup = this.onAddGroup.bind(this);
    this.onSearchGroups = this.onSearchGroups.bind(this);
    this.onSelectGroups = this.onSelectGroups.bind(this);
  }

  componentDidUpdate(prevProps, prevState) {
    if (this.props.match.params.type !== prevProps.match.params.type) {
      this.setState(this.mapPropsToState(this.props));
    }
  }

  mapPropsToState = (props) => {
    var groups = props.groups.map(item => {
      return {
        key: item.id,
        label: item.name,
        manager: item.manager,
        total: 0
      }
    });
    
    return {
      isLoading: false,
      errors: {
        firstName: false,
        lastName: false,
        email: false,
        password: false,
      },
      profile: toEmployeeWrapper({
        isVisitor: props.match.params.type === "guest",
        passwordType: "link"
      }),
      selector: {
        visible: false,
        allGroups: groups,
        groups: [...groups]
      }
    };
  }

  onInputChange(event) {
    var stateCopy = Object.assign({}, this.state);
    stateCopy.profile[event.target.name] = event.target.value;
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

  onGroupClose(id) {
    var stateCopy = Object.assign({}, this.state);
    stateCopy.profile.groups = this.state.profile.groups.filter((group) => group.id !== id);
    this.setState(stateCopy)
  }

  validate() {
    const { profile } = this.state;
    const emailRegex = /.+@.+\..+/;
    const errors = {
      firstName: !profile.firstName,
      lastName: !profile.lastName,
      email: !emailRegex.test(profile.email),
      password: profile.passwordType === "temp" && !profile.password
    };
    const hasError = errors.firstName || errors.lastName || errors.email || errors.password;
    this.setState({ errors: errors });
    return !hasError;
  }

  handleSubmit() {
    if (!this.validate())
      return false;

    this.setState({ isLoading: true });

    this.props.createProfile(this.state.profile)
      .then((profile) => {
        toastr.success("Success");
        this.props.history.push(`${this.props.settings.homepage}/view/${profile.userName}`);
      })
      .catch((error) => {
        toastr.error(error.message)
        this.setState({ isLoading: false })
      });
  }

  onCancel() {
    this.props.history.push(this.props.settings.homepage)
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

  onAddGroup() {
    var stateCopy = Object.assign({}, this.state);
    stateCopy.selector.visible = true;
    this.setState(stateCopy);
  }

  onSearchGroups(value) {
    var stateCopy = Object.assign({}, this.state);
    stateCopy.selector.groups = stateCopy.selector.allGroups.filter(item => {
      return value ? item.label.indexOf(value) > -1 : true;
    })
    this.setState(stateCopy);
  }

  onSelectGroups(value) {
    var stateCopy = Object.assign({}, this.state);
    var newGroups = value.map(item => {
      return {
        id: item.key,
        name: item.label,
        manager: item.manager
      };
    })
    stateCopy.profile.groups = [...stateCopy.profile.groups, ...newGroups];
    stateCopy.selector.groups = [...stateCopy.selector.allGroups];
    stateCopy.selector.visible = false;
    this.setState(stateCopy);
  }

  render() {
    const { isLoading, errors, profile, selector } = this.state;
    const { t, settings } = this.props;

    const pattern = getUserContactsPattern();
    const contacts = getUserContacts(profile.contacts);

    return (
      <>
        <MainContainer>
          <AvatarContainer>
            <Avatar
              size="max"
              role={getUserRole(profile)}
              editing={true}
              editLabel={t("AddPhoto")}
            />
          </AvatarContainer>
          <MainFieldsContainer>
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
            <TextField
              isRequired={true}
              hasError={errors.email}
              labelText={`${t("Email")}:`}
              inputName="email"
              inputValue={profile.email}
              inputIsDisabled={isLoading}
              inputOnChange={this.onInputChange}
              inputTabIndex={3}
            />
            <PasswordField
              isRequired={true}
              hasError={errors.password}
              labelText={`${t("Password")}:`}
              radioName="passwordType"
              radioValue={profile.passwordType}
              radioOptions={[
                { value: 'link', label: t("ActivationLink") },
                { value: 'temp', label: t("TemporaryPassword") }
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
            />
            <DateField
              labelText={`${t("Birthdate")}:`}
              inputName="birthday"
              inputValue={profile.birthday ? new Date(profile.birthday) : undefined}
              inputIsDisabled={isLoading}
              inputOnChange={this.onBirthdayDateChange}
              inputTabIndex={5}
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
            <DateField
              labelText={`${t("CustomEmployedSinceDate", { employedSinceDate })}:`}
              inputName="workFrom"
              inputValue={profile.workFrom ? new Date(profile.workFrom) : undefined}
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
              labelText={`${t("CustomPosition", { position })}:`}
              inputName="title"
              inputValue={profile.title}
              inputIsDisabled={isLoading}
              inputOnChange={this.onInputChange}
              inputTabIndex={8}
            />
            <DepartmentField
              labelText={`${t("CustomDepartment", { department })}:`}
              isDisabled={isLoading}
              departments={profile.groups}
              addButtonTitle={t("AddButton")}
              onAddDepartment={this.onAddGroup}
              onRemoveDepartment={this.onGroupClose}
            />
            <AdvancedSelector
              isDropDown={true}
              isOpen={selector.visible}
              maxHeight={336}
              width={379}
              placeholder={t("Search")}
              onSearchChanged={this.onSearchGroups}
              options={selector.groups}
              isMultiSelect={true}
              buttonLabel={t("Add departments")}
              selectAllLabel={t("Select all")}
              onSelect={this.onSelectGroups}
            />
          </MainFieldsContainer>
        </MainContainer>
        <InfoFieldContainer headerText={t("Comments")}>
          <Textarea name="notes" value={profile.notes} isDisabled={isLoading} onChange={this.onInputChange} tabIndex={9}/> 
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
          /> 
        </InfoFieldContainer>
        <div>
          <Button label={t("SaveButton")} onClick={this.handleSubmit} primary isDisabled={isLoading} size="big" tabIndex={10} />
          <Button label={t("CancelButton")} onClick={this.onCancel} isDisabled={isLoading} size="big" style={{ marginLeft: "8px" }} tabIndex={11} />
        </div>
      </>
    );
  };
}

const mapStateToProps = (state) => {
  return {
    settings: state.auth.settings,
    groups: state.people.groups,
  }
};

export default connect(
  mapStateToProps,
  {
    createProfile
  }
)(withRouter(withTranslation()(CreateUserForm)));