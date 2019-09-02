import React from 'react'
import { withRouter } from 'react-router'
import { connect } from 'react-redux'
import { Avatar, Button, Textarea, Text, toastr, ModalDialog } from 'asc-web-components'
import { withTranslation } from 'react-i18next';
import { toEmployeeWrapper, getUserRole, updateProfile } from '../../../../../store/profile/actions';
import { MainContainer, AvatarContainer, MainFieldsContainer } from './FormFields/Form'
import TextField from './FormFields/TextField'
import TextChangeField from './FormFields/TextChangeField'
import DateField from './FormFields/DateField'
import RadioField from './FormFields/RadioField'
import DepartmentField from './FormFields/DepartmentField'
import { departmentName, position, employedSinceDate, guest, employee } from '../../../../customNames';

class UpdateUserForm extends React.Component {

  constructor(props) {
    super(props);

    this.state = this.mapPropsToState(props);

    this.validate = this.validate.bind(this);
    this.handleSubmit = this.handleSubmit.bind(this);
    this.onTextChange = this.onTextChange.bind(this);
    this.onBirthdayDateChange = this.onBirthdayDateChange.bind(this);
    this.onWorkFromDateChange = this.onWorkFromDateChange.bind(this);
    this.onGroupClose = this.onGroupClose.bind(this);
    this.onCancel = this.onCancel.bind(this);

    this.onDialogShow = this.onDialogShow.bind(this);
    this.onDialogClose = this.onDialogClose.bind(this);
  }

  componentDidUpdate(prevProps, prevState) {
    if (this.props.match.params.userId !== prevProps.match.params.userId) {
      this.setState(this.mapPropsToState(this.props));
    }
  }

  mapPropsToState = (props) => {
    return {
      isLoading: false,
      isDialogVisible: false,
      errors: {
        firstName: false,
        lastName: false,
        email: false,
        password: false,
      },
      profile: toEmployeeWrapper(props.profile)
    };
  }

  onTextChange(event) {
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
    stateCopy.profile.groups = this.state.groups.filter((group) => group.id !== id);
    this.setState(stateCopy)
  }

  validate() {
    const errors = {
      firstName: !this.state.profile.firstName,
      lastName: !this.state.profile.lastName,
      email: !this.state.profile.email,
      password: this.state.profile.passwordType === "temp" && !this.state.profile.password
    };
    const hasError = errors.firstName || errors.lastName || errors.email || errors.password;
    this.setState({errors: errors});
    return !hasError;
  }

  handleSubmit() {
    if(!this.validate())
      return false;

    this.setState({isLoading: true});

    this.props.updateProfile(this.state.profile)
      .then((profile) => {
        toastr.success("Success");
        this.props.history.push(`${this.props.settings.homepage}/view/${profile.userName}`);
      })
      .catch((error) => {
        toastr.error(error.message)
        this.setState({isLoading: false})
      });
  }

  onCancel() {
    this.props.history.goBack();
  }

  onDialogShow() {
    this.setState({isDialogVisible: true})
  }

  onDialogClose() {
    this.setState({isDialogVisible: false})
  }

  render() {
    return (
      <>
        <MainContainer>
          <AvatarContainer>
            <Avatar
              size="max"
              role={getUserRole(this.state.profile)}
              source={this.state.profile.avatarMax}
              userName={this.state.profile.displayName}
              editing={true}
              editLabel={this.props.t("EditPhoto")}
            />
          </AvatarContainer>
          <MainFieldsContainer>
            <TextChangeField
              labelText={`${this.props.t("Email")}:`}
              inputName="email"
              inputValue={this.state.profile.email}
              buttonText={this.props.t("ChangeButton")}
              buttonIsDisabled={this.state.isLoading}
              buttonOnClick={this.onDialogShow}
            />
            <TextChangeField
              labelText={`${this.props.t("Password")}:`}
              inputName="password"
              inputValue={this.state.profile.password}
              buttonText={this.props.t("ChangeButton")}
              buttonIsDisabled={this.state.isLoading}
              buttonOnClick={this.onDialogShow}
            />
            <TextChangeField
              labelText={`${this.props.t("Phone")}:`}
              inputName="phone"
              inputValue={this.state.profile.phone}
              buttonText={this.props.t("ChangeButton")}
              buttonIsDisabled={this.state.isLoading}
              buttonOnClick={this.onDialogShow}
            />
            <TextField
              isRequired={true}
              hasError={this.state.errors.firstName}
              labelText={`${this.props.t("FirstName")}:`}
              inputName="firstName"
              inputValue={this.state.profile.firstName}
              inputIsDisabled={this.state.isLoading}
              inputOnChange={this.onTextChange}
            />
            <TextField
              isRequired={true}
              hasError={this.state.errors.lastName}
              labelText={`${this.props.t("LastName")}:`}
              inputName="lastName"
              inputValue={this.state.profile.lastName}
              inputIsDisabled={this.state.isLoading}
              inputOnChange={this.onTextChange}
            />
            <DateField
              labelText={`${this.props.t("Birthdate")}:`}
              inputName="birthday"
              inputValue={this.state.profile.birthday ? new Date(this.state.profile.birthday) : undefined}
              inputIsDisabled={this.state.isLoading}
              inputOnChange={this.onBirthdayDateChange}
            />
            <RadioField
              labelText={`${this.props.t("Sex")}:`}
              radioName="sex"
              radioValue={this.state.profile.sex}
              radioOptions={[
                { value: 'male', label: this.props.t("SexMale")},
                { value: 'female', label: this.props.t("SexFemale")}
              ]}
              radioIsDisabled={this.state.isLoading}
              radioOnChange={this.onTextChange}
            />
            <RadioField
              labelText={`${this.props.t("Type")}:`}
              radioName="sex"
              radioValue={this.state.profile.isVisitor.toString()}
              radioOptions={[
                { value: "true", label: this.props.t("CustomTypeGuest", { guest })},
                { value: "false", label: this.props.t("CustomTypeUser", { employee })}
              ]}
              radioIsDisabled={this.state.isLoading}
              radioOnChange={this.onTextChange}
            />
            <DateField
              labelText={`${this.props.t("CustomEmployedSinceDate", { employedSinceDate })}:`}
              inputName="workFrom"
              inputValue={this.state.profile.workFrom ? new Date(this.state.profile.workFrom) : undefined}
              inputIsDisabled={this.state.isLoading}
              inputOnChange={this.onWorkFromDateChange}
            />
            <TextField
              labelText={`${this.props.t("Location")}:`}
              inputName="location"
              inputValue={this.state.profile.location}
              inputIsDisabled={this.state.isLoading}
              inputOnChange={this.onTextChange}
            />
            <TextField
              labelText={`${this.props.t("CustomPosition", { position })}:`}
              inputName="title"
              inputValue={this.state.profile.title}
              inputIsDisabled={this.state.isLoading}
              inputOnChange={this.onTextChange}
            />
            <DepartmentField
              labelText={`${this.props.t("CustomDepartmentName", { departmentName })}:`}
              departments={this.state.profile.groups}
              onRemoveDepartment={this.onGroupClose}
            />
          </MainFieldsContainer>
        </MainContainer>
        <div>
          <Text.ContentHeader>{this.props.t("Comments")}</Text.ContentHeader>
          <Textarea name="notes" value={this.state.profile.notes} isDisabled={this.state.isLoading} onChange={this.onTextChange}/> 
        </div>
        <div style={{marginTop: "60px"}}>
          <Button label={this.props.t("SaveButton")} onClick={this.handleSubmit} primary isDisabled={this.state.isLoading} size="big"/>
          <Button label={this.props.t("CancelButton")} onClick={this.onCancel} isDisabled={this.state.isLoading} size="big" style={{ marginLeft: "8px" }}/>
        </div>

        <ModalDialog
          visible={this.state.isDialogVisible}
          headerContent={"Change something"}
          bodyContent={<p>Send the something instructions?</p>}
          footerContent={<Button label="Send" primary={true} onClick={this.onDialogClose} />}
          onClose={this.onDialogClose}
        />
      </>
    );
  };
}

const mapStateToProps = (state) => {
  return {
    profile: state.profile.targetUser,
    settings: state.auth.settings
  }
};

export default connect(
  mapStateToProps,
  {
    updateProfile
  }
)(withRouter(withTranslation()(UpdateUserForm)));