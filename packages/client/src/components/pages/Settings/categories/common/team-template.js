import React from "react";
import { inject, observer } from "mobx-react";
import styled from "styled-components";
import { withTranslation } from "react-i18next";
import ComboBox from "@docspace/components/combobox";
import TextInput from "@docspace/components/text-input";
import FieldContainer from "@docspace/components/field-container";
import toastr from "@docspace/components/toast/toastr";
import SaveCancelButtons from "@docspace/components/save-cancel-buttons";
import Loader from "@docspace/components/loader";
import { saveToSessionStorage, getFromSessionStorage } from "../../utils";
import { desktop } from "@docspace/components/utils/device";
import { Consumer } from "@docspace/components/utils/context";

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

  @media ${desktop} {
    .team-template_buttons {
      position: fixed;
      left: auto;
      padding-left: 0px;
      padding-right: 16px;
      max-width: ${(props) => props.sectionWidth}px;
    }
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

//Use for title t("CustomTitles") and for tooltip t("TeamTemplateSettingsDescription")

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
let isError;
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
      isLoadingData: false,
      isChanged: false,
      availableOptions: [],
      customId: "custom",
      customName: "",
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
    const { getCustomSchemaList, showReminder, setIsLoading } = this.props;

    const { customNames } = this.props;

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
    if (
      !userFromSessionStorage &&
      !usersFromSessionStorage &&
      !groupFromSessionStorage &&
      !groupsFromSessionStorage &&
      !jobFromSessionStorage &&
      !registrationDateFromSessionStorage &&
      !groupLeadFromSessionStorage &&
      !guestFromSessionStorage &&
      !guestsFromSessionStorage &&
      selectedOptionFromSessionStorage !== customNames.name
    ) {
      this.setState({
        selectedOption: {
          key: 0,
          label: customNames.name,
        },
      });
      saveToSessionStorage("selectedOption", customNames.name);
    }
    setIsLoading(true);
    getCustomSchemaList()
      .then(() => this.getOptions())
      .then(() => setIsLoading(false));
  }

  onCustomSchemaSelect = (option) => {
    const { customSchemaList } = this.props;

    const currentTemplate = customSchemaList[option.key];

    if (isError) {
      this.setState({
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
      });
    }

    if (this.settingIsEqualInitialValue("id", currentTemplate.id)) {
      this.setState({ isChanged: false });
    } else this.setState({ isChanged: true });

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

    const fieldsArray = this.getFields(currentTemplate);
    const keysArray = Object.keys(currentTemplate);

    saveToSessionStorage("selectedOption", currentTemplate.name);

    for (let i = 0; i < keysArray.length; i++) {
      if (
        this.settingIsEqualInitialValue(`${keysArray[i]}`, `${fieldsArray[i]}`)
      ) {
        saveToSessionStorage(`${keysArray[i]}`, "");
      } else {
        saveToSessionStorage(`${keysArray[i]}`, `${fieldsArray[i]}`);
      }
    }
  };

  getFields = (obj) => {
    return Object.keys(obj).reduce((acc, rec) => {
      return [...acc, obj[rec]];
    }, []);
  };

  getOptions = () => {
    const { customSchemaList } = this.props;
    const { customId } = this.state;

    for (let item = 0; item < customSchemaList.length; item++) {
      let obj = {
        key: item,
        label: customSchemaList[item].name,
        disabled: false,
      };
      options.push(obj);

      if (customSchemaList[item].id === customId) {
        this.setState({
          customName: customSchemaList[item].name,
        });
      }
    }
    this.setState({ availableOptions: options });
  };

  settingIsEqualInitialValue = (settingName, value) => {
    const { customNames } = this.props;

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
      ) {
        isChanged = true;
      }
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
    const { selectedOption, customName, customId } = this.state;

    const name = e.target.name;
    const value = e.target.value;

    this.setState({ [name]: value });

    if (selectedOption.label !== customName) {
      this.setState({
        id: customId,
        selectedOption: {
          key: 0,
          label: customName,
        },
      });
      saveToSessionStorage("selectedOption", customName);
    }

    if (this.settingIsEqualInitialValue(`${name}`, `${value}`)) {
      saveToSessionStorage(`${name}`, "");
    } else {
      saveToSessionStorage(`${name}`, `${value}`);
    }

    if (value.length === 0) {
      this.setState({
        isChanged: true,
      });
    } else this.checkChanges();
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

    isError =
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

  onSaveClick = () => {
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
      customName,
    } = this.state;

    const {
      setCurrentSchema,
      setCustomSchema,
      getCurrentCustomSchema,
      t,
    } = this.props;

    if (this.isInvalidForm()) return;

    if (selectedOption.label !== customName) {
      this.setState({ isLoadingData: true }, function () {
        setCurrentSchema(id)
          .then(() => getCurrentCustomSchema(id))

          .then(() => toastr.success(t("SuccessfullySaveSettingsMessage")))
          .catch((error) => toastr.error(error))
          .finally(() => this.setState({ isLoadingData: false }));
      });
    } else {
      this.setState({ isLoadingData: true }, function () {
        setCustomSchema(
          userCaption,
          usersCaption,
          groupCaption,
          groupsCaption,
          userPostCaption,
          regDateCaption,
          groupHeadCaption,
          guestCaption,
          guestsCaption
        )
          .then(() => getCurrentCustomSchema(id))
          .then(() => toastr.success(t("SuccessfullySaveSettingsMessage")))
          .catch((error) => toastr.error(error))
          .finally(() => this.setState({ isLoadingData: false }));
      });
    }

    settingNames.forEach((settingName) => {
      saveToSessionStorage(settingName, "");
    });

    this.setState({
      showReminder: false,
      isChanged: false,
    });
  };
  onCancelClick = () => {
    const { customNames } = this.props;
    settingNames.forEach((settingName) => {
      const defaultValue = customNames[settingName];
      this.setState({ [settingName]: defaultValue });
      saveToSessionStorage(settingName, "");
    });

    saveToSessionStorage("selectedOption", customNames.name);

    if (isError) {
      this.setState({
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
      });
    }
    this.setState({
      showReminder: false,
      isChanged: false,
      id: customNames.id,
      selectedOption: {
        key: 0,
        label: customNames.name,
      },
    });
  };
  render() {
    const { t, isLoading } = this.props;

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
      availableOptions,
      isLoadingData,
    } = this.state;

    return isLoading ? (
      <Loader className="pageLoader" type="rombs" size="40px" />
    ) : (
      <Consumer>
        {(context) => (
          <StyledComponent sectionWidth={context.sectionWidth}>
            <FieldContainer
              className="team-template_field-container main-field-container"
              isVertical
              labelText={t("Template")}
              place="top"
            >
              <ComboBox
                options={availableOptions}
                selectedOption={selectedOption}
                onSelect={this.onCustomSchemaSelect}
                isDisabled={isLoadingData}
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
              labelText={t("Common:User")}
              place="top"
              hasError={formErrors.userCaption}
              errorMessage={t("Common:EmptyFieldError")}
            >
              <TextInput
                name={"userCaption"}
                className="team-template_text-input"
                scale={true}
                value={userCaption}
                hasError={formErrors.userCaption}
                onChange={this.onChangeInput}
                isDisabled={isLoadingData}
                placeholder={t("AddName")}
                tabIndex={1}
              />
            </FieldContainer>

            <FieldContainer
              className="team-template_field-container"
              isVertical
              labelText={t("Users")}
              place="top"
              hasError={formErrors.usersCaption}
              errorMessage={t("Common:EmptyFieldError")}
            >
              <TextInput
                name={"usersCaption"}
                className="team-template_text-input"
                scale={true}
                value={usersCaption}
                hasError={formErrors.usersCaption}
                onChange={this.onChangeInput}
                placeholder={t("AddName")}
                isDisabled={isLoadingData}
                tabIndex={2}
              />
            </FieldContainer>

            <FieldContainer
              className="team-template_field-container"
              isVertical
              labelText={t("Group")}
              place="top"
              hasError={formErrors.groupCaption}
              errorMessage={t("Common:EmptyFieldError")}
            >
              <TextInput
                name={"groupCaption"}
                className="team-template_text-input"
                scale={true}
                value={groupCaption}
                hasError={formErrors.groupCaption}
                onChange={this.onChangeInput}
                placeholder={t("AddName")}
                isDisabled={isLoadingData}
                tabIndex={3}
              />
            </FieldContainer>

            <FieldContainer
              className="team-template_field-container"
              isVertical
              labelText={t("Groups")}
              place="top"
              hasError={formErrors.groupsCaption}
              errorMessage={t("Common:EmptyFieldError")}
            >
              <TextInput
                name={"groupsCaption"}
                className="team-template_text-input"
                scale={true}
                value={groupsCaption}
                hasError={formErrors.groupsCaption}
                onChange={this.onChangeInput}
                placeholder={t("AddName")}
                isDisabled={isLoadingData}
                tabIndex={4}
              />
            </FieldContainer>

            <FieldContainer
              className="team-template_field-container"
              isVertical
              labelText={t("Job/Title")}
              place="top"
              hasError={formErrors.userPostCaption}
              errorMessage={t("Common:EmptyFieldError")}
            >
              <TextInput
                name={"userPostCaption"}
                className="team-template_text-input"
                scale={true}
                value={userPostCaption}
                hasError={formErrors.userPostCaption}
                onChange={this.onChangeInput}
                placeholder={t("AddName")}
                isDisabled={isLoadingData}
                tabIndex={5}
              />
            </FieldContainer>

            <FieldContainer
              className="team-template_field-container"
              isVertical
              labelText={t("RegistrationDate")}
              place="top"
              hasError={formErrors.regDateCaption}
              errorMessage={t("Common:EmptyFieldError")}
            >
              <TextInput
                name={"regDateCaption"}
                className="team-template_text-input"
                scale={true}
                value={regDateCaption}
                hasError={formErrors.regDateCaption}
                onChange={this.onChangeInput}
                placeholder={t("AddName")}
                isDisabled={isLoadingData}
                tabIndex={6}
              />
            </FieldContainer>

            <FieldContainer
              className="team-template_field-container"
              isVertical
              labelText={t("GroupLead")}
              place="top"
              hasError={formErrors.groupHeadCaption}
              errorMessage={t("Common:EmptyFieldError")}
            >
              <TextInput
                name={"groupHeadCaption"}
                className="team-template_text-input"
                scale={true}
                value={groupHeadCaption}
                hasError={formErrors.groupHeadCaption}
                onChange={this.onChangeInput}
                placeholder={t("AddName")}
                isDisabled={isLoadingData}
                tabIndex={7}
              />
            </FieldContainer>

            <FieldContainer
              className="team-template_field-container"
              isVertical
              labelText={t("Common:Guest")}
              place="top"
              hasError={formErrors.guestCaption}
              errorMessage={t("Common:EmptyFieldError")}
            >
              <TextInput
                name={"guestCaption"}
                className="team-template_text-input"
                scale={true}
                value={guestCaption}
                hasError={formErrors.guestCaption}
                onChange={this.onChangeInput}
                placeholder={t("AddName")}
                isDisabled={isLoadingData}
                tabIndex={8}
              />
            </FieldContainer>

            <FieldContainer
              className="team-template_field-container"
              isVertical
              labelText={t("Guests")}
              place="top"
              hasError={formErrors.guestsCaption}
              errorMessage={t("Common:EmptyFieldError")}
            >
              <TextInput
                name={"guestsCaption"}
                className="team-template_text-input"
                scale={true}
                value={guestsCaption}
                hasError={formErrors.guestsCaption}
                onChange={this.onChangeInput}
                placeholder={t("AddName")}
                isDisabled={isLoadingData}
                tabIndex={9}
              />
            </FieldContainer>

            {isChanged && (
              <SaveCancelButtons
                className="team-template_buttons"
                onSaveClick={this.onSaveClick}
                onCancelClick={this.onCancelClick}
                showReminder={showReminder}
                reminderTest={t("YouHaveUnsavedChanges")}
                saveButtonLabel={t("Common:SaveButton")}
                cancelButtonLabel={t("Common:CancelButton")}
              />
            )}
          </StyledComponent>
        )}
      </Consumer>
    );
  }
}

export default inject(({ auth, setup }) => {
  const {
    nameSchemaId,
    organizationName,

    getCustomSchemaList,

    customNames,
    customSchemaList,
    isLoading,
    getCurrentCustomSchema,
    setIsLoading,
  } = auth.settingsStore;
  const { setCurrentSchema, setCustomSchema } = setup;
  return {
    nameSchemaId,
    organizationName,

    getCustomSchemaList,
    getCurrentCustomSchema,
    customNames,
    customSchemaList,
    isLoading,
    setCurrentSchema,
    setCustomSchema,
    setIsLoading,
  };
})(withTranslation(["Settings", "Common"])(observer(TeamTemplate)));
