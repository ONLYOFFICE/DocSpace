import React from "react";
import { inject, observer } from "mobx-react";
import styled from "styled-components";
import { withTranslation } from "react-i18next";
import ComboBox from "@appserver/components/combobox";
import TextInput from "@appserver/components/text-input";
import FieldContainer from "@appserver/components/field-container";
import toastr from "@appserver/components/toast/toastr";
import SaveCancelButtons from "@appserver/components/save-cancel-buttons";
import { isMobile } from "react-device-detect";

const StyledComponent = styled.div`
  .team-template_text-input {
    max-width: 350px;
  }

  .team-template_field-container {
    margin-bottom: 8px;
  }
  .main-field-container {
    margin-bottom: 32px;
  }
  .team-template_buttons {
    position: absolute;

    justify-content: space-between;
    ${isMobile &&
    `
      max-width: 500px;
      position: static;
      padding-left: 0px;
 
    `}
  }
  .cancel-button {
    margin-right: 16px;
  }
`;
let options = [];
class TeamTemplate extends React.Component {
  constructor(props) {
    super(props);

    const { customNames, t } = this.props;
    options = [];
    this.state = {
      selectedOption: { key: 0, label: `${customNames.name}` },
      id: customNames.id,
      user: customNames.userCaption,
      users: customNames.usersCaption,
      group: customNames.groupCaption,
      groups: customNames.groupsCaption,
      job: customNames.userPostCaption,
      registrationDate: customNames.regDateCaption,
      groupLead: customNames.groupHeadCaption,
      guest: customNames.guestCaption,
      guests: customNames.guestsCaption,
      showReminder: false,
      isLoading: false,
      hasChanged: false,
      formErrors: {
        user: false,
        users: false,
        group: false,
        groups: false,
        job: false,
        registrationDate: false,
        groupLead: false,
        guest: false,
        guests: false,
      },
    };
  }

  componentDidMount() {
    const { getCustomSchema, t } = this.props;

    getCustomSchema().then(() => this.getOptions());
  }

  onCustomSchemaSelect = (option) => {
    console.log("select", option);
    const { teamTemplate } = this.props;
    const currentTemplate = teamTemplate[option.key];

    this.setState({
      selectedOption: {
        key: 0,
        label: currentTemplate.name,
      },
      id: currentTemplate.id,
      user: currentTemplate.userCaption,
      users: currentTemplate.usersCaption,
      group: currentTemplate.groupCaption,
      groups: currentTemplate.groupsCaption,
      job: currentTemplate.userPostCaption,
      registrationDate: currentTemplate.regDateCaption,
      groupLead: currentTemplate.groupHeadCaption,
      guest: currentTemplate.guestCaption,
      guests: currentTemplate.guestsCaption,
    });
  };

  getOptions = () => {
    const { isLoading, teamTemplate } = this.props;
    //debugger;
    console.log("isLoading", isLoading);

    for (let item = 0; item < teamTemplate.length; item++) {
      let obj = {
        key: item,
        label: teamTemplate[item].name,
        disabled: false,
      };
      options.push(obj);
    }
  };

  onChangeInput = (e) => {
    //debugger;
    const { teamTemplate } = this.props;
    const { selectedOption } = this.state;

    const name = e.target.name;
    const value = e.target.value;

    this.setState({ [name]: value });
    console.log("change");

    selectedOption.label !== teamTemplate[3].name &&
      this.setState({
        id: teamTemplate[3].id,
        selectedOption: {
          key: 0,
          label: teamTemplate[3].name,
        },
      });
  };
  isInvalidForm = () => {
    const {
      user,
      users,
      group,
      groups,
      registrationDate,
      groupLead,
      job,
      guest,
      guests,
    } = this.state;

    const errors = {
      user: !user.trim(),
      users: !users.trim(),
      group: !group.trim(),
      groups: !groups.trim(),
      job: !job.trim(),
      registrationDate: !registrationDate.trim(),
      groupLead: !groupLead.trim(),
      guest: !guest.trim(),
      guests: !guests.trim(),
    };

    const isError =
      errors.user ||
      errors.users ||
      errors.group ||
      errors.groups ||
      errors.job ||
      errors.registrationDate ||
      errors.groupLead ||
      errors.guest ||
      errors.guests;

    this.setState({ formErrors: errors });

    return isError;
  };

  onSaveSettings = () => {
    const {
      id,
      user,
      users,
      group,
      groups,
      registrationDate,
      groupLead,
      job,
      guest,
      guests,
      selectedOption,
    } = this.state;

    const {
      setCurrentShema,
      setCustomShema,
      teamTemplate,
      getCurrentCustomSchema,
      t,
    } = this.props;

    if (this.isInvalidForm()) return;

    this.setState({ isLoading: true });
    //debugger;
    if (selectedOption.label !== teamTemplate[3].name) {
      setCurrentShema(id)
        .then(() => getCurrentCustomSchema(id))
        .then(() => toastr.success(t("SuccessfullySaveSettingsMessage")))
        .catch((error) => toastr.error(error));
    } else {
      setCustomShema(
        user,
        users,
        group,
        groups,
        registrationDate,
        groupLead,
        job,
        guest,
        guests
      )
        .then(() => getCurrentCustomSchema(id))
        .then(() => toastr.success(t("SuccessfullySaveSettingsMessage")))
        .catch((error) => toastr.error(error));
    }
  };
  render() {
    const { t } = this.props;

    console.log("options", options);
    const {
      user,
      users,
      group,
      groups,
      registrationDate,
      groupLead,
      job,
      guest,
      guests,
      selectedOption,
      formErrors,
    } = this.state;

    return (
      <StyledComponent>
        <FieldContainer
          className="team-template_field-container main-field-container"
          isVertical
          labelText={`${t("Template")}:`}
          place="top"
        >
          <ComboBox
            options={options}
            selectedOption={selectedOption}
            onSelect={this.onCustomSchemaSelect}
            isDisabled={false}
            noBorder={false}
            scaled={true}
            scaledOptions={true}
            dropDownMaxHeight={300}
            className="team-template_text-input"
          />
        </FieldContainer>

        <FieldContainer
          className="team-template_field-container"
          isVertical
          labelText={`${t("User")}:`}
          place="top"
          hasError={formErrors.user}
        >
          <TextInput
            name={"user"}
            className="team-template_text-input"
            scale={true}
            value={user}
            hasError={formErrors.user}
            onChange={this.onChangeInput}
            isDisabled={false}
            placeholder={t("AddName")}
          />
        </FieldContainer>

        <FieldContainer
          className="team-template_field-container"
          isVertical
          labelText={`${t("Users")}:`}
          place="top"
          hasError={formErrors.users}
        >
          <TextInput
            name={"users"}
            className="team-template_text-input"
            scale={true}
            value={users}
            hasError={formErrors.users}
            onChange={this.onChangeInput}
            isDisabled={false}
            placeholder={t("AddName")}
          />
        </FieldContainer>

        <FieldContainer
          className="team-template_field-container"
          isVertical
          labelText={`${t("Group")}:`}
          place="top"
          hasError={formErrors.group}
        >
          <TextInput
            name={"group"}
            className="team-template_text-input"
            scale={true}
            value={group}
            hasError={formErrors.group}
            onChange={this.onChangeInput}
            isDisabled={false}
            placeholder={t("AddName")}
          />
        </FieldContainer>

        <FieldContainer
          className="team-template_field-container"
          isVertical
          labelText={`${t("Groups")}:`}
          place="top"
          hasError={formErrors.groups}
        >
          <TextInput
            name={"groups"}
            className="team-template_text-input"
            scale={true}
            value={groups}
            hasError={formErrors.groups}
            onChange={this.onChangeInput}
            isDisabled={false}
            placeholder={t("AddName")}
          />
        </FieldContainer>

        <FieldContainer
          className="team-template_field-container"
          isVertical
          labelText={`${t("Job/Title")}:`}
          place="top"
          hasError={formErrors.job}
        >
          <TextInput
            name={"job"}
            className="team-template_text-input"
            scale={true}
            value={job}
            hasError={formErrors.job}
            onChange={this.onChangeInput}
            isDisabled={false}
            placeholder={t("AddName")}
          />
        </FieldContainer>

        <FieldContainer
          className="team-template_field-container"
          isVertical
          labelText={`${t("RegistrationDate")}:`}
          place="top"
          hasError={formErrors.registrationDate}
        >
          <TextInput
            name={"registrationDate"}
            className="team-template_text-input"
            scale={true}
            value={registrationDate}
            hasError={formErrors.registrationDate}
            onChange={this.onChangeInput}
            isDisabled={false}
            placeholder={t("AddName")}
          />
        </FieldContainer>

        <FieldContainer
          className="team-template_field-container"
          isVertical
          labelText={`${t("GroupLead")}:`}
          place="top"
          hasError={formErrors.groupLead}
        >
          <TextInput
            name={"groupLead"}
            className="team-template_text-input"
            scale={true}
            value={groupLead}
            hasError={formErrors.groupLead}
            onChange={this.onChangeInput}
            isDisabled={false}
            placeholder={t("AddName")}
          />
        </FieldContainer>

        <FieldContainer
          className="team-template_field-container"
          isVertical
          labelText={`${t("Guest")}:`}
          place="top"
          hasError={formErrors.guest}
        >
          <TextInput
            name={"guest"}
            className="team-template_text-input"
            scale={true}
            value={guest}
            hasError={formErrors.guest}
            onChange={this.onChangeInput}
            isDisabled={false}
            placeholder={t("AddName")}
          />
        </FieldContainer>

        <FieldContainer
          className="team-template_field-container"
          isVertical
          labelText={`${t("Guests")}:`}
          place="top"
          hasError={formErrors.guests}
        >
          <TextInput
            name={"guests"}
            className="team-template_text-input"
            scale={true}
            value={guests}
            hasError={formErrors.guests}
            onChange={this.onChangeInput}
            isDisabled={false}
            placeholder={t("AddName")}
          />
        </FieldContainer>

        <SaveCancelButtons
          className="team-template_buttons"
          onSaveClick={this.onSaveSettings}
          onCancelClick={() => console.log("cancel")}
          showReminder={true}
          reminderTest={t("YouHaveUnsavedChanges")}
          saveButtonLabel={t("SaveButton")}
          cancelButtonLabel={t("CancelButton")}
        />
      </StyledComponent>
    );
  }
}

export default inject(({ auth, setup }) => {
  const {
    nameSchemaId,
    organizationName,

    getCustomSchema,

    customNames,
    teamTemplate,
    isLoading,
    getCurrentCustomSchema,
  } = auth.settingsStore;
  const { setCurrentShema, setCustomShema } = setup;
  return {
    nameSchemaId,
    organizationName,

    getCustomSchema,
    getCurrentCustomSchema,
    customNames,
    teamTemplate,
    isLoading,
    setCurrentShema,
    setCustomShema,
  };
})(withTranslation("Settings")(observer(TeamTemplate)));
