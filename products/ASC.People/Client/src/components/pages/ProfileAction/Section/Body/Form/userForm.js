import React, { useCallback } from 'react'
import { withRouter } from 'react-router'
import { connect } from 'react-redux'
import { Field, reduxForm, formValueSelector } from 'redux-form'
import { Avatar, Button, TextInput, Textarea, DateInput, Label, RadioButton, Text } from 'asc-web-components'
import submit from './submit'
import validate from './validate'
import styled from 'styled-components';
import { useTranslation } from 'react-i18next';

const getUserRole = user => {
  if(user.isOwner) return "owner";
  if(user.isAdmin) return "admin";
  if(user.isVisitor) return "guest";
  return "user";
};

const onEditAvatar = () => {};

const size = {
  mobile: "375px",
  tablet: "768px",
  desktop: "1024px"
};

const device = {
  mobile: `(max-width: ${size.mobile})`,
  tablet: `(max-width: ${size.tablet})`,
  desktop: `(max-width: ${size.desktop})`
};

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
)

const renderPasswordField = ({ input, isDisabled }) => (
  <TextInput {...input} type="password" autoComplete="new-password" className="field-input" isDisabled={isDisabled} />
)

const renderDateField = ({ input, label, isRequired, meta: { touched, error } }) => (
  <FieldContainer>
    <Label isRequired={isRequired} error={!!(touched && error)} text={label} className="field-label"/>
    <FieldBody>
      <DateInput {...input}/>
    </FieldBody>
  </FieldContainer>
)

const renderRadioField = ({ input, label, isChecked }) => (
  <RadioButton {...input} label={label} isChecked={isChecked}/> 
)

let UserForm = props => {
  const { t, i18n } = useTranslation();
  const { error, handleSubmit, submitting, initialValues, sexIsMale, passwordTypeIsLink, passwordError, userType, history } = props;

  const onCancel = useCallback(() => {
    history.goBack();
  }, [history]);

  return (
    <form onSubmit={handleSubmit(submit)}>
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
                editLabel={t('Resource:EditPhoto')}
                editAction={onEditAvatar}
              />
            : <Avatar
                size="max"
                role={userType}
                editing={true}
                editLabel={t('Resource:AddPhoto')}
                editAction={onEditAvatar}
              />
          }
        </AvatarContainer>
        <MainFieldsContainer>
          <Field
            name="firstName"
            component={renderTextField}
            label={`${t('Resource:FirstName')}:`}
            isRequired={true}
          />
          <Field
            name="lastName"
            component={renderTextField}
            label={`${t('Resource:LastName')}:`}
            isRequired={true}
          />
          <Field
            name="email"
            component={renderTextField}
            label={`${t('Resource:Email')}:`}
            isRequired={true}
          />

          <FieldContainer>
            <Label text={`${t('Resource:Password')}:`} isRequired={true} error={passwordError} className="field-label"/>
            <FieldBody>
              <RadioGroupFieldBody>
                <Field component={renderRadioField} type="radio" name="passwordType" value="link" label={t('Resource:ActivationLink')} isChecked={passwordTypeIsLink}/>
                <Field component={renderRadioField} type="radio" name="passwordType" value="temp" label={t('Resource:TemporaryPassword')} isChecked={!passwordTypeIsLink}/>
              </RadioGroupFieldBody>
              <Field
                name="password"
                component={renderPasswordField}
                isDisabled={passwordTypeIsLink}
              />
            </FieldBody>
          </FieldContainer>

          <Field
            name="birthDate"
            component={renderDateField}
            label={`${t('Resource:Birthdate')}:`}
          />
          <FieldContainer>
            <Label text={`${t('Resource:Sex')}:`} className="field-label"/>
            <RadioGroupFieldBody>
              <Field component={renderRadioField} type="radio" name="sex" value="male" label={t('Resource:SexMale')} isChecked={sexIsMale}/>
              <Field component={renderRadioField} type="radio" name="sex" value="female" label={t('Resource:SexFemale')} isChecked={!sexIsMale}/>
            </RadioGroupFieldBody>
          </FieldContainer>
          <Field
            name="workFrom"
            component={renderDateField}
            label={`${t('Resource:EmployedSinceDate')}:`}
          />
          <Field
            name="location"
            component={renderTextField}
            label={`${t('Resource:Location')}:`}
          />
          <Field
            name="position"
            component={renderTextField}
            label={`${t('Resource:Position')}:`}
          />
        </MainFieldsContainer>
      </MainContainer>
      <div>
        <Text.ContentHeader>{t('Resource:Comments')}</Text.ContentHeader>
        <Field component={Textarea} name="comments" />
      </div>
      <div>
        {error && <strong>{error}</strong>}
      </div>
      <div style={{marginTop: "60px"}}>
        <Button label={t('UserControlsCommonResource:SaveButton')} primary type="submit" isDisabled={submitting}/>
        <Button label={t('UserControlsCommonResource:CancelButton')} style={{ marginLeft: '8px' }} isDisabled={submitting} onClick={onCancel}/>
      </div>
    </form>
  )
}

UserForm = reduxForm({
  validate,
  form: 'userForm',
  enableReinitialize: true
})(withRouter(UserForm))

const selector = formValueSelector('userForm')

UserForm = connect(
  state => {

    const sexIsMale = selector(state, 'sex') == 'male';

    const passwordTypeIsLink = selector(state, 'passwordType') == 'link';

    const passwordValue = selector(state, 'password');
    
    const passwordError = state &&
      state.form &&
      state.form.userForm &&
      state.form.userForm.fields &&
      state.form.userForm.fields.password &&
      state.form.userForm.fields.password.touched && 
      !passwordValue;

    const fixDate = (name) => {
      const workFromIsDate = state &&
          state.form &&
          state.form.userForm &&
          state.form.userForm.values &&
          state.form.userForm.values[name] &&
          state.form.userForm.values[name] instanceof Date;

        if(workFromIsDate)
        {
          let date = state.form.userForm.values[name];
          let str = new Date(date.getTime() - (date.getTimezoneOffset() * 60000)).toJSON()
          state.form.userForm.values[name] = str;
        }
    }

    fixDate("workFrom");
    fixDate("birthDate");

    return {
      sexIsMale,
      passwordTypeIsLink,
      passwordError
    }
  }
)(UserForm)

export default UserForm
