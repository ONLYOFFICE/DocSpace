import React from "react";
import { withTranslation } from "react-i18next";
import styled from "styled-components";
import FieldContainer from "@appserver/components/field-container";
import Loader from "@appserver/components/loader";
import toastr from "@appserver/components/toast/toastr";
import TextInput from "@appserver/components/text-input";
import Link from "@appserver/components/link";
import SaveCancelButtons from "@appserver/components/save-cancel-buttons";
import { showLoader, hideLoader } from "@appserver/common/utils";
import { saveToSessionStorage, getFromSessionStorage } from "../../utils";
import { setDocumentTitle } from "../../../../../helpers/utils";
import { inject, observer } from "mobx-react";

const StyledComponent = styled.div`
  .margin-top {
    margin-top: 20px;
  }

  .margin-left {
    margin-left: 20px;
  }

  .settings-block {
    margin-bottom: 70px;
  }

  .field-container-width {
    max-width: 500px;
  }

  .combo-button-label {
    max-width: 100%;
  }
  .link-wrapper {
    margin-top: 8px;
  }
`;

let greetingTitleFromSessionStorage = "";

const settingNames = ["greetingTitle"];

class CustomTitles extends React.Component {
  constructor(props) {
    super(props);

    const { t, greetingSettings /*, organizationName*/ } = props;

    greetingTitleFromSessionStorage = getFromSessionStorage("greetingTitle");

    setDocumentTitle(t("Customization"));

    this.state = {
      isLoadedData: false,
      isLoading: false,
      greetingTitle: greetingTitleFromSessionStorage || greetingSettings,
      greetingTitleDefault: greetingSettings,
      isLoadingGreetingSave: false,
      isLoadingGreetingRestore: false,
      hasChanged: false,
      showReminder: false,
    };
  }

  componentDidMount() {
    const { showReminder } = this.state;
    showLoader();
    if (greetingTitleFromSessionStorage && !showReminder) {
      this.setState({
        showReminder: true,
      });
    }
    this.setState({
      isLoadedData: true,
    });
    hideLoader();
  }

  componentDidUpdate(prevProps, prevState) {
    if (prevState.isLoadedData !== true) {
      this.setState({
        isLoadedData: true,
      });
    }

    if (this.state.greetingTitleDefault) {
      this.checkChanges();
    }
  }

  onChangeGreetingTitle = (e) => {
    this.setState({ greetingTitle: e.target.value });

    if (this.settingIsEqualInitialValue("greetingTitle", e.target.value)) {
      saveToSessionStorage("greetingTitle", "");
    } else {
      saveToSessionStorage("greetingTitle", e.target.value);
    }

    this.checkChanges();
  };

  onSaveGreetingSettings = () => {
    const { setGreetingTitle, t } = this.props;
    const { greetingTitle } = this.state;
    this.setState({ isLoadingGreetingSave: true }, function () {
      setGreetingTitle(greetingTitle)
        .then(() =>
          toastr.success(t("SuccessfullySaveGreetingSettingsMessage"))
        )
        .catch((error) => toastr.error(error))
        .finally(() => this.setState({ isLoadingGreetingSave: false }));
    });

    this.setState({
      showReminder: false,
      greetingTitle: greetingTitle,
      greetingTitleDefault: greetingTitle,
    });
  };

  onRestoreGreetingSettings = () => {
    const { restoreGreetingTitle, t } = this.props;
    this.setState({ isLoadingGreetingRestore: true }, function () {
      restoreGreetingTitle()
        .then(() => {
          this.setState({
            greetingTitle: this.props.greetingSettings,
            greetingTitleDefault: this.props.greetingSettings,
            showReminder: false,
          });
          saveToSessionStorage("greetingTitle", "");
          toastr.success(t("SuccessfullySaveGreetingSettingsMessage"));
        })
        .catch((error) => toastr.error(error))
        .finally(() => this.setState({ isLoadingGreetingRestore: false }));
    });
  };

  onCancelClick = () => {
    settingNames.forEach((currentSetting) => {
      const valueFromSessionStorage = getFromSessionStorage(currentSetting);

      if (
        valueFromSessionStorage &&
        !this.settingIsEqualInitialValue(
          currentSetting,
          valueFromSessionStorage
        )
      ) {
        const defaultValue = this.state[currentSetting + "Default"];

        this.setState({ [currentSetting]: defaultValue });
        saveToSessionStorage(currentSetting, "");
      }
    });

    this.setState({
      showReminder: false,
    });

    this.checkChanges();
  };

  settingIsEqualInitialValue = (stateName, value) => {
    const defaultValue = JSON.stringify(this.state[stateName + "Default"]);
    const currentValue = JSON.stringify(value);
    return defaultValue === currentValue;
  };

  checkChanges = () => {
    let hasChanged = false;

    settingNames.forEach((settingName) => {
      const valueFromSessionStorage = getFromSessionStorage(settingName);
      if (
        valueFromSessionStorage &&
        !this.settingIsEqualInitialValue(settingName, valueFromSessionStorage)
      )
        hasChanged = true;
    });

    if (hasChanged !== this.state.hasChanged) {
      this.setState({
        hasChanged: hasChanged,
      });
    }
  };

  render() {
    const { t } = this.props;
    const {
      isLoadedData,
      greetingTitle,
      isLoadingGreetingSave,
      isLoadingGreetingRestore,
      hasChanged,
      showReminder,
    } = this.state;

    return !isLoadedData ? (
      <Loader className="pageLoader" type="rombs" size="40px" />
    ) : (
      <>
        <StyledComponent>
          <div className="settings-block">
            <FieldContainer
              id="fieldContainerWelcomePage"
              className="field-container-width"
              labelText={`${t("WelcomePageTitle")}:`}
              isVertical={true}
            >
              <TextInput
                scale={true}
                value={greetingTitle}
                onChange={this.onChangeGreetingTitle}
                isDisabled={isLoadingGreetingSave || isLoadingGreetingRestore}
              />
              <div className="link-wrapper">
                <Link
                  onClick={this.onRestoreGreetingSettings}
                  type="action"
                  color="#A3A9AE"
                >
                  {t("SetDefaultTitle")}
                </Link>
              </div>
            </FieldContainer>
          </div>
          {hasChanged && (
            <SaveCancelButtons
              onSaveClick={this.onSaveGreetingSettings}
              onCancelClick={this.onCancelClick}
              showReminder={showReminder}
              reminderTest={t("YouHaveUnsavedChanges")}
              saveButtonLabel={t("Common:SaveButton")}
              cancelButtonLabel={t("Common:CancelButton")}
            />
          )}
        </StyledComponent>
      </>
    );
  }
}

export default inject(({ auth, setup }) => {
  const { greetingSettings, organizationName } = auth.settingsStore;
  const { setGreetingTitle, restoreGreetingTitle } = setup;
  return {
    greetingSettings,
    organizationName,
    setGreetingTitle,
    restoreGreetingTitle,
  };
})(withTranslation(["Settings", "Common"])(observer(CustomTitles)));
