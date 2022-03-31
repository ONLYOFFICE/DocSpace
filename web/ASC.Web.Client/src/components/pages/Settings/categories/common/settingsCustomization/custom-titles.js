import React from "react";
import { withTranslation } from "react-i18next";
import styled, { css } from "styled-components";
import FieldContainer from "@appserver/components/field-container";
import Loader from "@appserver/components/loader";
import toastr from "@appserver/components/toast/toastr";
import TextInput from "@appserver/components/text-input";
import HelpButton from "@appserver/components/help-button";
import SaveCancelButtons from "@appserver/components/save-cancel-buttons";
import { showLoader, hideLoader } from "@appserver/common/utils";
import { saveToSessionStorage, getFromSessionStorage } from "../../../utils";
import { setDocumentTitle } from "../../../../../../helpers/utils";
import { inject, observer } from "mobx-react";
import { CustomTitlesTooltip } from "../sub-components/common-tooltips";
import { LANGUAGE } from "@appserver/common/constants";
import { convertLanguage } from "@appserver/common/utils";
import withCultureNames from "@appserver/common/hoc/withCultureNames";
import { LanguageTimeSettingsTooltip } from "../sub-components/common-tooltips";
import { combineUrl } from "@appserver/common/utils";
import { AppServerConfig } from "@appserver/common/constants";
import config from "../../../../../../../package.json";
import history from "@appserver/common/history";
import { isMobileOnly } from "react-device-detect";
import Scrollbar from "@appserver/components/scrollbar";
import Text from "@appserver/components/text";
import Box from "@appserver/components/box";
import Link from "@appserver/components/link";
import ArrowRightIcon from "../../../../../../../public/images/arrow.right.react.svg";
import { isSmallTablet } from "@appserver/components/utils/device";
import commonIconsStyles from "@appserver/components/utils/common-icons-style";
import { Base } from "@appserver/components/themes";
import checkScrollSettingsBlock from "../utils";

const menuHeight = "48px";
const sectionHeight = "50px";
const paddingSectionWrapperContent = "22px";
const saveCancelButtons = "56px";
const flex = "4px";

const StyledArrowRightIcon = styled(ArrowRightIcon)`
  ${commonIconsStyles}
  path {
    fill: ${(props) => props.theme.studio.settings.common.arrowColor};
  }
`;

StyledArrowRightIcon.defaultProps = { theme: Base };

const StyledScrollbar = styled(Scrollbar)`
  height: calc(
    100vh -
      (
        ${menuHeight} + ${sectionHeight} + ${paddingSectionWrapperContent} +
          ${saveCancelButtons} + ${flex}
      )
  ) !important;
  width: 100% !important;
`;

const StyledComponent = styled.div`
  .settings-block {
    margin-bottom: 70px;
  }

  .settings-block {
    max-width: 350px;
  }

  .combo-button-label {
    max-width: 100%;
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
      hasScroll: false,
    };
  }

  componentDidMount() {
    const { showReminder } = this.state;
    window.addEventListener("resize", this.checkInnerWidth);
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
    const { hasScroll } = this.state;
    const checkScroll = checkScrollSettingsBlock();

    window.addEventListener("resize", checkScroll);
    const scrollLngTZSettings = checkScroll();

    if (scrollLngTZSettings !== hasScroll) {
      this.setState({
        hasScroll: scrollLngTZSettings,
      });
    }

    // TODO: Remove div with height 64 and remove settings-mobile class
    const settingsMobile = document.getElementsByClassName(
      "settings-mobile"
    )[0];

    if (settingsMobile) {
      settingsMobile.style.display = "none";
    }

    if (prevState.isLoadedData !== true) {
      this.setState({
        isLoadedData: true,
      });
    }

    if (this.state.greetingTitleDefault) {
      this.checkChanges();
    }
  }

  componentWillUnmount() {
    window.removeEventListener(
      "resize",
      this.checkInnerWidth,
      checkScrollSettingsBlock
    );
  }

  onChangeGreetingTitle = (e) => {
    this.setState({ greetingTitle: e.target.value });

    if (this.settingIsEqualInitialValue("greetingTitle", e.target.value)) {
      saveToSessionStorage("greetingTitle", "");
    } else {
      saveToSessionStorage("greetingTitle", e.target.value);
      this.setState({
        showReminder: true,
      });
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

  checkInnerWidth = () => {
    if (!isSmallTablet()) {
      history.push(
        combineUrl(
          AppServerConfig.proxyURL,
          config.homepage,
          "/settings/common/customization"
        )
      );
      return true;
    }
  };

  render() {
    const { t, theme, sectionWidth, isMobileView } = this.props;
    const {
      isLoadedData,
      greetingTitle,
      isLoadingGreetingSave,
      isLoadingGreetingRestore,
      showReminder,
      hasChanged,
      hasScroll,
    } = this.state;

    const tooltipCustomTitlesTooltip = <CustomTitlesTooltip t={t} />;

    const isMobileViewLanguageTimeSettings = (
      <div className="category-item-wrapper">
        <div className="category-item-heading">
          <Link
            truncate={true}
            className="inherit-title-link header"
            onClick={this.onClickLink}
            href={combineUrl(
              AppServerConfig.proxyURL,
              "/settings/common/customization/custom-titles"
            )}
          >
            {t("CustomTitles")}
          </Link>
          <StyledArrowRightIcon size="small" color="#333333" />
        </div>
        <Text className="category-item-description">
          {t("CustomTitlesSettingsDescription")}
        </Text>
      </div>
    );

    const settingsBlock = (
      <div className="settings-block">
        <FieldContainer
          id="fieldContainerWelcomePage"
          className="field-container-width"
          labelText={t("Common:Title")}
          isVertical={true}
        >
          <TextInput
            scale={true}
            value={greetingTitle}
            onChange={this.onChangeGreetingTitle}
            isDisabled={isLoadingGreetingSave || isLoadingGreetingRestore}
            placeholder={`${t("Cloud Office Applications")}`}
          />
        </FieldContainer>
      </div>
    );

    return !isLoadedData ? (
      <Loader className="pageLoader" type="rombs" size="40px" />
    ) : isMobileView ? (
      isMobileViewLanguageTimeSettings
    ) : (
      <StyledComponent hasScroll={hasScroll}>
        {this.checkInnerWidth() && !isMobileView && (
          <div className="category-item-heading">
            <div className="category-item-title">{t("WelcomePageTitle")}</div>
            <HelpButton
              iconName="static/images/combined.shape.svg"
              size={12}
              tooltipContent={tooltipCustomTitlesTooltip}
            />
          </div>
        )}
        {(isMobileOnly && isSmallTablet()) || isSmallTablet() ? (
          <StyledScrollbar stype="smallBlack">{settingsBlock}</StyledScrollbar>
        ) : (
          <> {settingsBlock}</>
        )}
        <SaveCancelButtons
          className="save-cancel-buttons"
          onSaveClick={this.onSaveGreetingSettings}
          onCancelClick={this.onRestoreGreetingSettings}
          showReminder={showReminder}
          reminderTest={t("YouHaveUnsavedChanges")}
          saveButtonLabel={t("Common:SaveButton")}
          cancelButtonLabel={t("Settings:RestoreDefaultButton")}
          displaySettings={true}
          hasScroll={hasScroll}
        />
      </StyledComponent>
    );
  }
}

export default inject(({ auth, setup }) => {
  const { greetingSettings, organizationName, theme } = auth.settingsStore;
  const { setGreetingTitle, restoreGreetingTitle } = setup;
  return {
    theme,
    greetingSettings,
    organizationName,
    setGreetingTitle,
    restoreGreetingTitle,
  };
})(withTranslation(["Settings", "Common"])(observer(CustomTitles)));
