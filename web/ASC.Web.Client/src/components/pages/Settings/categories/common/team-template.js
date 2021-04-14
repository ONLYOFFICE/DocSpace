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
import { saveToSessionStorage, getFromSessionStorage } from "../../utils";

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
let userFromSessionStorage,
  usersFromSessionStorage,
  groupFromSessionStorage,
  groupsFromSessionStorage,
  jobFromSessionStorage,
  registrationDateFromSessionStorage,
  groupLeadFromSessionStorage,
  guestFromSessionStorage,
  guestsFromSessionStorage,
  selectedOptionFromSessionStorage;

const settingNames = [
  "userCaption",
  "usersCaption",
  "groupCaption",
  "groupsCaption",
  "userPostCaption",
  "regDateCaption",
  "groupHeadCaption",
  "guestCaption",
  "guestsCaption",
];
class TeamTemplate extends React.Component {
  constructor(props) {
    super(props);

    const { customNames, t } = this.props;
    options = [];

    userFromSessionStorage = getFromSessionStorage("userCaption");
    usersFromSessionStorage = getFromSessionStorage("usersCaption");
    groupFromSessionStorage = getFromSessionStorage("groupCaption");
    groupsFromSessionStorage = getFromSessionStorage("groupsCaption");
    jobFromSessionStorage = getFromSessionStorage("userPostCaption");
    registrationDateFromSessionStorage = getFromSessionStorage(
      "regDateCaption"
    );
    groupLeadFromSessionStorage = getFromSessionStorage("groupHeadCaption");
    guestFromSessionStorage = getFromSessionStorage("guestCaption");
    guestsFromSessionStorage = getFromSessionStorage("guestsCaption");
    selectedOptionFromSessionStorage = getFromSessionStorage("selectedOption");

    this.state = {
      selectedOption: (selectedOptionFromSessionStorage && {
        key: 0,
        label: selectedOptionFromSessionStorage,
      }) || {
        key: 0,
        label: `${t(customNames.name)}`,
      },
      id: customNames.id,
      userCaption: userFromSessionStorage || customNames.userCaption,
      usersCaption: usersFromSessionStorage || customNames.usersCaption,
      groupCaption: groupFromSessionStorage || customNames.groupCaption,
      groupsCaption: groupsFromSessionStorage || customNames.groupsCaption,
      userPostCaption: jobFromSessionStorage || customNames.userPostCaption,
      regDateCaption:
        registrationDateFromSessionStorage || customNames.regDateCaption,
      groupHeadCaption:
        groupLeadFromSessionStorage || customNames.groupHeadCaption,
      guestCaption: guestFromSessionStorage || customNames.guestCaption,
      guestsCaption: guestsFromSessionStorage || customNames.guestsCaption,

      showReminder: false,
      isLoading: false,
      isChanged: false,
      formErrors: {
        userCaption: false,
        usersCaption: false,
        groupCaption: false,
        groupsCaption: false,
        userPostCaption: false,
        regDateCaption: false,
        groupHeadCaption: false,
        guestCaption: false,
        guestsCaption: false,
      },
    };
  }

  componentDidMount() {
    const { getCustomSchema, showReminder } = this.props;
    const { isChanged } = this.props;

    //isChanged && this.setState()
    //debugger;
    if (
      userFromSessionStorage ||
      usersFromSessionStorage ||
      groupFromSessionStorage ||
      groupsFromSessionStorage ||
      jobFromSessionStorage ||
      registrationDateFromSessionStorage ||
      groupLeadFromSessionStorage ||
      guestFromSessionStorage ||
      guestsFromSessionStorage
    ) {
      this.checkChanges();
      !showReminder &&
        this.setState({
          showReminder: true,
        });
    }

    getCustomSchema().then(() => this.getOptions());
  }

  onCustomSchemaSelect = (option) => {
    console.log("select", option);

    const { teamTemplate } = this.props;

    const currentTemplate = teamTemplate[option.key];

    if (this.settingIsEqualInitialValue("id", currentTemplate.id))
      this.setState({ isChanged: false });
    else this.setState({ isChanged: true });

    this.setState({
      selectedOption: {
        key: 0,
        label: currentTemplate.name,
      },
      id: currentTemplate.id,
      userCaption: currentTemplate.userCaption,
      usersCaption: currentTemplate.usersCaption,
      groupCaption: currentTemplate.groupCaption,
      groupsCaption: currentTemplate.groupsCaption,
      userPostCaption: currentTemplate.userPostCaption,
      regDateCaption: currentTemplate.regDateCaption,
      groupHeadCaption: currentTemplate.groupHeadCaption,
      guestCaption: currentTemplate.guestCaption,
      guestsCaption: currentTemplate.guestsCaption,
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

  settingIsEqualInitialValue = (settingName, value) => {
    const { customNames } = this.props;
    //debugger;
    const defaultValue = customNames[settingName];
    const currentValue = value;
    return defaultValue === currentValue;
  };

  checkChanges = () => {
    const { customNames } = this.props;
    let isChanged = false;

    settingNames.forEach((settingName) => {
      const valueFromSessionStorage = getFromSessionStorage(settingName);
      if (
        valueFromSessionStorage &&
        !this.settingIsEqualInitialValue(settingName, valueFromSessionStorage)
      )
        isChanged = true;
    });

    if (!isChanged) {
      this.setState({
        id: customNames.id,
        selectedOption: {
          key: 0,
          label: customNames.name,
        },
      });
      saveToSessionStorage("selectedOption", customNames.name);
    }

    if (isChanged !== this.state.isChanged) {
      this.setState({
        isChanged: isChanged,
      });
    }
  };

  onChangeInput = (e) => {
    //debugger;
    const { teamTemplate, customNames } = this.props;
    const { selectedOption } = this.state;

    const name = e.target.name;
    const value = e.target.value;

    this.setState({ [name]: value });

    console.log("change");

    if (selectedOption.label !== teamTemplate[3].name) {
      this.setState({
        id: teamTemplate[3].id,
        selectedOption: {
          key: 0,
          label: teamTemplate[3].name,
        },
      });
      saveToSessionStorage("selectedOption", teamTemplate[3].name);
    }

    if (this.settingIsEqualInitialValue(`${name}`, `${value}`)) {
      saveToSessionStorage(`${name}`, "");
    } else {
      saveToSessionStorage(`${name}`, `${value}`);
    }

    this.checkChanges();
  };
  isInvalidForm = () => {
    const {
      userCaption,
      usersCaption,
      groupCaption,
      groupsCaption,
      regDateCaption,
      groupHeadCaption,
      userPostCaption,
      guestCaption,
      guestsCaption,
    } = this.state;

    const errors = {
      userCaption: !userCaption.trim(),
      usersCaption: !usersCaption.trim(),
      groupCaption: !groupCaption.trim(),
      groupsCaption: !groupsCaption.trim(),
      userPostCaption: !userPostCaption.trim(),
      regDateCaption: !regDateCaption.trim(),
      groupHeadCaption: !groupHeadCaption.trim(),
      guestCaption: !guestCaption.trim(),
      guestsCaption: !guestsCaption.trim(),
    };

    const isError =
      errors.userCaption ||
      errors.usersCaption ||
      errors.groupCaption ||
      errors.groupsCaption ||
      errors.userPostCaption ||
      errors.regDateCaption ||
      errors.groupHeadCaption ||
      errors.guestCaption ||
      errors.guestsCaption;

    this.setState({ formErrors: errors });

    return isError;
  };

  onSaveSettings = () => {
    const {
      id,
      userCaption,
      usersCaption,
      groupCaption,
      groupsCaption,
      regDateCaption,
      groupHeadCaption,
      userPostCaption,
      guestCaption,
      guestsCaption,
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

    //this.setState({ isLoading: true });
    //debugger;
    if (selectedOption.label !== teamTemplate[3].name) {
      setCurrentShema(id)
        .then(() => getCurrentCustomSchema(id))
        .then(() => toastr.success(t("SuccessfullySaveSettingsMessage")))
        .catch((error) => toastr.error(error));
    } else {
      setCustomShema(
        userCaption,
        usersCaption,
        groupCaption,
        groupsCaption,
        regDateCaption,
        groupHeadCaption,
        userPostCaption,
        guestCaption,
        guestsCaption
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
      userCaption,
      usersCaption,
      groupCaption,
      groupsCaption,
      regDateCaption,
      groupHeadCaption,
      userPostCaption,
      guestCaption,
      guestsCaption,
      selectedOption,
      formErrors,
      isChanged,
      showReminder,
    } = this.state;
    console.log("showReminder", showReminder);
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
          hasError={formErrors.userCaption}
        >
          <TextInput
            name={"userCaption"}
            className="team-template_text-input"
            scale={true}
            value={userCaption}
            hasError={formErrors.userCaption}
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
          hasError={formErrors.usersCaption}
        >
          <TextInput
            name={"usersCaption"}
            className="team-template_text-input"
            scale={true}
            value={usersCaption}
            hasError={formErrors.usersCaption}
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
          hasError={formErrors.groupCaption}
        >
          <TextInput
            name={"groupCaption"}
            className="team-template_text-input"
            scale={true}
            value={groupCaption}
            hasError={formErrors.groupCaption}
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
          hasError={formErrors.groupsCaption}
        >
          <TextInput
            name={"groupsCaption"}
            className="team-template_text-input"
            scale={true}
            value={groupsCaption}
            hasError={formErrors.groupsCaption}
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
          hasError={formErrors.userPostCaption}
        >
          <TextInput
            name={"userPostCaption"}
            className="team-template_text-input"
            scale={true}
            value={userPostCaption}
            hasError={formErrors.userPostCaption}
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
          hasError={formErrors.regDateCaption}
        >
          <TextInput
            name={"regDateCaption"}
            className="team-template_text-input"
            scale={true}
            value={regDateCaption}
            hasError={formErrors.regDateCaption}
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
          hasError={formErrors.groupHeadCaption}
        >
          <TextInput
            name={"groupHeadCaption"}
            className="team-template_text-input"
            scale={true}
            value={groupHeadCaption}
            hasError={formErrors.groupHeadCaption}
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
          hasError={formErrors.guestCaption}
        >
          <TextInput
            name={"guestCaption"}
            className="team-template_text-input"
            scale={true}
            value={guestCaption}
            hasError={formErrors.guestCaption}
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
          hasError={formErrors.guestsCaption}
        >
          <TextInput
            name={"guestsCaption"}
            className="team-template_text-input"
            scale={true}
            value={guestsCaption}
            hasError={formErrors.guestsCaption}
            onChange={this.onChangeInput}
            isDisabled={false}
            placeholder={t("AddName")}
          />
        </FieldContainer>

        {isChanged && (
          <SaveCancelButtons
            className="team-template_buttons"
            onSaveClick={this.onSaveSettings}
            onCancelClick={() => console.log("cancel")}
            showReminder={showReminder}
            reminderTest={t("YouHaveUnsavedChanges")}
            saveButtonLabel={t("SaveButton")}
            cancelButtonLabel={t("CancelButton")}
          />
        )}
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
