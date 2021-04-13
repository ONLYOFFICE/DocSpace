import React from "react";
import { inject, observer } from "mobx-react";
import styled from "styled-components";
import { withTranslation } from "react-i18next";
import ComboBox from "@appserver/components/combobox";
import TextInput from "@appserver/components/text-input";
import FieldContainer from "@appserver/components/field-container";

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
      user: customNames.userCaption,
      users: customNames.usersCaption,
      group: customNames.groupCaption,
      groups: customNames.groupsCaption,
      job: customNames.userPostCaption,
      registrationDate: customNames.regDateCaption,
      groupLead: customNames.groupHeadCaption,
      guest: customNames.guestCaption,
      guests: customNames.guestsCaption,
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
    var stateCopy = { ...this.state };
    stateCopy[e.target.name] = e.target.value;
    this.setState(stateCopy);
    console.log("change");

    this.state.selectedOption.label !== teamTemplate[3].name &&
      this.setState({
        selectedOption: {
          key: 0,
          label: teamTemplate[3].name,
        },
      });
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
        >
          <TextInput
            name={"user"}
            className="team-template_text-input"
            scale={true}
            value={user}
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
        >
          <TextInput
            name={"users"}
            className="team-template_text-input"
            scale={true}
            value={users}
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
        >
          <TextInput
            name={"group"}
            className="team-template_text-input"
            scale={true}
            value={group}
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
        >
          <TextInput
            name={"groups"}
            className="team-template_text-input"
            scale={true}
            value={groups}
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
        >
          <TextInput
            name={"job"}
            className="team-template_text-input"
            scale={true}
            value={job}
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
        >
          <TextInput
            name={"registrationDate"}
            className="team-template_text-input"
            scale={true}
            value={registrationDate}
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
        >
          <TextInput
            name={"groupLead"}
            className="team-template_text-input"
            scale={true}
            value={groupLead}
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
        >
          <TextInput
            name={"guest"}
            className="team-template_text-input"
            scale={true}
            value={guest}
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
        >
          <TextInput
            name={"guests"}
            className="team-template_text-input"
            scale={true}
            value={guests}
            onChange={this.onChangeInput}
            isDisabled={false}
            placeholder={t("AddName")}
          />
        </FieldContainer>

        <SaveCancelButtons
          className="team-template_buttons"
          onSaveClick={() => console.log("save")}
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

export default inject(({ auth }) => {
  const {
    nameSchemaId,
    organizationName,

    getCustomSchema,

    customNames,
    teamTemplate,
    isLoading,
  } = auth.settingsStore;

  return {
    nameSchemaId,
    organizationName,

    getCustomSchema,
    customNames,
    teamTemplate,
    isLoading,
  };
})(withTranslation("Settings")(observer(TeamTemplate)));
