import React, { useCallback } from 'react'
import { withRouter } from 'react-router'
import { connect } from 'react-redux'
import { Field, FieldArray, reduxForm, formValueSelector } from 'redux-form'
import { device, Avatar, Button, TextInput, Textarea, DateInput, Label, RadioButton, Text, toastr, SelectedItem } from 'asc-web-components'
import styled from 'styled-components';
import { useTranslation } from 'react-i18next';
import { createProfile, updateProfile } from '../../../../../../store/profile/actions';

const formName = "userForm";

const getUserRole = user => {
  if(user.isOwner) return "owner";
  if(user.isAdmin) return "admin";
  if(user.isVisitor) return "guest";
  return "user";
};

const onEditAvatar = () => {};

const MainContainer = styled.div`
  display: flex;
  flex-direction: row;

  @media ${device.tablet} {
    flex-direction: column;
  }
`;

const AvatarContainer = styled.div`
  margin: 0 32px 32px 0;
  width: 160px;
`;

const MainFieldsContainer = styled.div`
  flex-grow: 1;
`;

const FieldContainer = styled.div`
  display: flex;
  flex-direction: row;
  margin: 0 0 16px 0;

  .field-label {
    line-height: 32px;
    margin: 0;
    width: 110px;
  }

  .field-input {
    width: 320px;
  }

  .radio-group {
    line-height: 32px;
    display: flex;

    label:not(:first-child) {
        margin-left: 33px;
    }
  }

  @media ${device.tablet} {
    flex-direction: column;
    align-items: start;

    .field-label {
      line-height: unset;
      margin: 0 0 4px 0;
      width: auto;
      flex-grow: 1;
    }
  }
`;

const FieldBody = styled.div`
  flex-grow: 1;
`;

const RadioGroupFieldBody = styled(FieldBody).attrs({
  className: "radio-group"
})``;

const renderTextField = ({ input, label, isRequired, meta: { touched, error } }) => (
  <FieldContainer>
    <Label isRequired={isRequired} error={!!(touched && error)} text={label} className="field-label"/>
    <FieldBody>
      <TextInput {...input} type="text" className="field-input"/>
    </FieldBody>
  </FieldContainer>
);

const renderPasswordField = ({ input, isDisabled }) => (
  <TextInput {...input} type="password" autoComplete="new-password" className="field-input" isDisabled={isDisabled} />
);

const renderDateField = ({ input, label, isRequired, meta: { touched, error } }) => (
  <FieldContainer>
    <Label isRequired={isRequired} error={touched && !!error} text={label} className="field-label"/>
    <FieldBody>
      <DateInput {...input} selected={input.value instanceof Date ? input.value : undefined}/>
    </FieldBody>
  </FieldContainer>
);

const renderRadioField = ({ input, label, isChecked }) => (
  <RadioButton {...input} label={label} isChecked={isChecked}/> 
);

const renderDepartmentField = ({ input, onClick }) => (
  <SelectedItem
    text={input.value}
    onClose={onClick}
    isInline={true}
    style={{ marginRight: "8px", marginBottom: "8px" }}
  />
);

const renderDepartmentsField = ({ fields, label }) => {
  return (
    <FieldContainer>
      <Label text={label} className="field-label"/>
      <FieldBody>
        {fields.map((member, index) => (      
          <Field
            key={`${member}${index}`}  
            name={`${member}.name`}
            component={renderDepartmentField}
            onClick={() => fields.remove(index)}
          />
        ))}
      </FieldBody>
    </FieldContainer>
  )
};

const renderTextareaField = ({ input }) => (
  <Textarea {...input} /> 
);

let UserForm = (props) => {
  const { t } = useTranslation();
  const {
    error,
    handleSubmit,
    submitting,
    initialValues,
    sexValue,
    passwordTypeValue,
    passwordError,
    userType,
    history,
    updateProfile,
    createProfile
  } = props;

  const employeeWrapperToMemberModel = (profile) => {
    const comment = profile.notes;
    const department = profile.groups ? profile.groups.map(group => group.id) : [];
    const worksFrom = profile.workFrom;

    return {...profile, comment, department, worksFrom};
  }

  const onCancel = useCallback(() => {
    history.goBack();
  }, [history]);

  const onSubmit = useCallback(async (values) => {
      try {
        const member = employeeWrapperToMemberModel(values);

        if (values.id) {
          await updateProfile(member);
        } else {
          await createProfile(member);
        }

        toastr.success("Success");
        history.goBack();
      } catch(error) {
        toastr.error(error.message);
      }
  }, [history, updateProfile, createProfile]);

  return (
    <form onSubmit={handleSubmit(onSubmit)}>
      <MainContainer>
        <AvatarContainer>
          {
            initialValues.id
            ? <Avatar
                size="max"
                role={getUserRole(initialValues)}
                source={initialValues.avatarMax}
                userName={initialValues.displayName}
                editing={true}
                editLabel={t("Edit Photo")}
                editAction={onEditAvatar}
              />
            : <Avatar
                size="max"
                role={userType}
                editing={true}
                editLabel={t("AddPhoto")}
                editAction={onEditAvatar}
              />
          }
        </AvatarContainer>
        <MainFieldsContainer>
          <Field
            name="firstName"
            component={renderTextField}
            label={`${t("FirstName")}:`}
            isRequired={true}
          />
          <Field
            name="lastName"
            component={renderTextField}
            label={`${t("LastName")}:`}
            isRequired={true}
          />
          <Field
            name="email"
            component={renderTextField}
            label={`${t("Email")}:`}
            isRequired={true}
          />
          <FieldContainer>
            <Label
              text={`${t("Password")}:`}
              isRequired={true}
              error={passwordError}
              className="field-label"
            />
            <FieldBody>
              <RadioGroupFieldBody>
                <Field
                  component={renderRadioField}
                  type="radio"
                  name="passwordType"
                  value="link"
                  label={t("ActivationLink")}
                  isChecked={passwordTypeValue === "link"}
                />
                <Field
                  component={renderRadioField}
                  type="radio"
                  name="passwordType"
                  value="temp"
                  label={t("TemporaryPassword")}
                  isChecked={passwordTypeValue === "temp"}
                />
              </RadioGroupFieldBody>
              <Field
                name="password"
                component={renderPasswordField}
                isDisabled={passwordTypeValue === "link"}
              />
            </FieldBody>
          </FieldContainer>
          <Field
            name="birthday"
            component={renderDateField}
            label={`${t("Birthdate")}:`}
          />
          <FieldContainer>
            <Label
              text={`${t("Sex")}:`}
              className="field-label"
            />
            <RadioGroupFieldBody>
              <Field
                component={renderRadioField}
                type="radio"
                name="sex"
                value="male"
                label={t("SexMale")}
                isChecked={sexValue === "male"}
              />
              <Field
                component={renderRadioField}
                type="radio"
                name="sex"
                value="female"
                label={t("SexFemale")}
                isChecked={sexValue === "female"}
              />
            </RadioGroupFieldBody>
          </FieldContainer>
          <Field
            name="workFrom"
            component={renderDateField}
            label={`EmployedSinceDate:`}
          />
          <Field
            name="location"
            component={renderTextField}
            label={`${t("Location")}:`}
          />
          <Field
            name="title"
            component={renderTextField}
            label={`Position:`}
          />
          <FieldArray
            name="groups"
            component={renderDepartmentsField}
            label={`Departments`}
          />
        </MainFieldsContainer>
      </MainContainer>
      <div>
        <Text.ContentHeader>{t("Comments")}</Text.ContentHeader>
        <Field 
          name="notes"
          component={renderTextareaField}
        />
      </div>
      <div>
        {error && <strong>{error}</strong>}
      </div>
      <div style={{marginTop: "60px"}}>
        <Button
          label={t("SaveButton")}
          primary
          type="submit"
          isDisabled={submitting}
        />
        <Button
          label={t("CancelButton")}
          style={{ marginLeft: "8px" }}
          isDisabled={submitting}
          onClick={onCancel}
        />
      </div>
    </form>
  )
}

const validate = (values) => {
  const requiredFieldText = "RequiredField";
  const errors = {};

  if (!values.firstName)
    errors.firstName = requiredFieldText;

  if (!values.lastName)
    errors.lastName = requiredFieldText;

  if (!values.email)
    errors.email = requiredFieldText;

  if (values.passwordType === "temp" && !values.password)
    errors.password = requiredFieldText;

  return errors
};

UserForm = reduxForm({
  validate,
  form: formName,
  enableReinitialize: true,
})(withRouter(UserForm))

const selector = formValueSelector(formName)

const mapStateToProps = (state) => {
    const sexValue = selector(state, "sex") || "male";
    const passwordTypeValue = selector(state, "passwordType") || "link";
    const passwordError = 
      passwordTypeValue !== "link" &&
      state &&
      state.form &&
      state.form.userForm &&
      state.form.userForm.fields &&
      state.form.userForm.fields.password &&
      state.form.userForm.fields.password.touched && 
      !selector(state, "password");

    return {
      sexValue,
      passwordTypeValue,
      passwordError
    }
};

export default connect(
  mapStateToProps,
  {
    createProfile,
    updateProfile
  }
)(UserForm)