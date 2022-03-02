import React from "react";
import { withTranslation } from "react-i18next";
import styled from "styled-components";
import Text from "@appserver/components/text";
import Box from "@appserver/components/box";
import toastr from "@appserver/components/toast/toastr";
import Link from "@appserver/components/link";
import ArrowRightIcon from "../../../../../../public/images/arrow.right.react.svg";
import { setDocumentTitle } from "../../../../../helpers/utils";
import commonIconsStyles from "@appserver/components/utils/common-icons-style";
import { showLoader, hideLoader, combineUrl } from "@appserver/common/utils";
import { inject, observer } from "mobx-react";
import { AppServerConfig } from "@appserver/common/constants";
import withCultureNames from "@appserver/common/hoc/withCultureNames";
import LanguageAndTimeZone from "./language-and-time-zone";
import CustomTitles from "./custom-titles";
import PortalRenaming from "./portal-renaming";
import WhiteLabel from "./whitelabel";
import { Base } from "@appserver/components/themes";
import { Consumer } from "@appserver/components/utils/context";

const mapTimezonesToArray = (timezones) => {
  return timezones.map((timezone) => {
    return { key: timezone.id, label: timezone.displayName };
  });
};

const findSelectedItemByKey = (items, selectedItemKey) => {
  return items.find((item) => item.key === selectedItemKey);
};

const StyledArrowRightIcon = styled(ArrowRightIcon)`
  ${commonIconsStyles}
  path {
    fill: ${(props) => props.theme.studio.settings.common.arrowColor};
  }
`;

StyledArrowRightIcon.defaultProps = { theme: Base };

const StyledComponent = styled.div`
  .margin-top {
    margin-top: 20px;
  }

  .margin-left {
    margin-left: 20px;
  }

  .settings-block {
    margin-bottom: 24px;
  }

  .field-container-width {
    max-width: 500px;
  }

  .combo-button-label {
    max-width: 100%;
  }

  .category-item-wrapper {
    /* .category-item-heading {
      display: flex;
      align-items: center;
      margin-bottom: 16px;
    } */

    .category-item-subheader {
      font-size: 13px;
      font-weight: 600;
      margin-bottom: 5px;
    }

    .category-item-description {
      color: ${(props) => props.theme.studio.settings.common.descriptionColor};
      font-size: 12px;
      max-width: 1024px;
    }

    /* .inherit-title-link {
      margin-right: 7px;
      font-size: 19px;
      font-weight: 600;
    } */

    .link-text {
      margin: 0;
    }

    /* .category-item-title {
      font-weight: bold;
      font-size: 16px;
      line-height: 22px;
      margin-right: 4px;
    } */
  }
`;

StyledComponent.defaultProps = { theme: Base };
class Customization extends React.Component {
  constructor(props) {
    super(props);

    const {
      portalLanguage,
      portalTimeZoneId,
      cultureNames,
      rawTimezones,
      /*organizationName,*/
      t,
    } = props;
    const timezones = mapTimezonesToArray(rawTimezones);

    setDocumentTitle(t("Customization"));

    this.state = {
      isLoading: false,
      timezones,
      timezone: findSelectedItemByKey(
        timezones,
        portalTimeZoneId || timezones[0]
      ),
      language: findSelectedItemByKey(
        cultureNames,
        portalLanguage || cultureNames[0]
      ),
      isLoadingGreetingSave: false,
      isLoadingGreetingRestore: false,
    };
  }

  componentDidMount() {
    const {
      portalLanguage,
      portalTimeZoneId,
      getPortalTimezones,
      cultureNames,
    } = this.props;

    const { timezones } = this.state;
    showLoader();
    if (!timezones.length && !cultureNames.length) {
      getPortalTimezones().then(() => {
        const timezones = mapTimezonesToArray(this.props.rawTimezones);
        const timezone =
          findSelectedItemByKey(timezones, portalTimeZoneId) || timezones[0];
        const language =
          findSelectedItemByKey(cultureNames, portalLanguage) ||
          cultureNames[0];

        this.setState({ language, timezones, timezone });
      });
    }

    hideLoader();
  }

  componentDidUpdate(prevProps) {
    const {
      i18n,
      language,
      nameSchemaId,
      getCurrentCustomSchema,
      cultureNames,
    } = this.props;

    if (language !== prevProps.language) {
      changeLanguage(i18n)
        .then(() => {
          const newLocaleSelectedLanguage =
            findSelectedItemByKey(cultureNames, this.state.language.key) ||
            cultureNames[0];

          this.setState({
            language: newLocaleSelectedLanguage,
          });
        })
        //.then(() => getModules(clientStore.dispatch))
        .then(() => getCurrentCustomSchema(nameSchemaId));
    }
  }

  onLanguageSelect = (language) => {
    this.setState({ language });
  };

  onTimezoneSelect = (timezone) => {
    this.setState({ timezone });
  };

  onSaveLngTZSettings = () => {
    const { setLanguageAndTime, i18n } = this.props;
    this.setState({ isLoading: true }, function () {
      setLanguageAndTime(this.state.language.key, this.state.timezone.key)
        .then(() => changeLanguage(i18n))
        .then((t) => toastr.success(t("SuccessfullySaveSettingsMessage")))
        .catch((error) => toastr.error(error))
        .finally(() => this.setState({ isLoading: false }));
    });
  };

  onClickLink = (e) => {
    e.preventDefault();
    const { history } = this.props;
    history.push(e.target.pathname);
  };

  render() {
    const { t, helpUrlCommonSettings, customNames, theme } = this.props;

    return (
      <Consumer>
        {(context) => (
          <StyledComponent>
            <div className="category-item-wrapper">
              <LanguageAndTimeZone sectionWidth={context.sectionWidth} />
            </div>
            <div className="category-item-wrapper">
              <CustomTitles sectionWidth={context.sectionWidth} />
            </div>
            <div className="category-item-wrapper">
              <PortalRenaming sectionWidth={context.sectionWidth} />
            </div>
            <WhiteLabel />
          </StyledComponent>
        )}
      </Consumer>
    );
  }
}

export default inject(({ auth, setup }) => {
  const {
    culture,
    timezone,
    timezones,
    nameSchemaId,
    organizationName,
    getCurrentCustomSchema,
    getPortalTimezones,
    helpUrlCommonSettings,
    customNames,
    theme,
  } = auth.settingsStore;

  const { setLanguageAndTime } = setup;

  return {
    theme,
    portalLanguage: culture,
    language: culture,
    portalTimeZoneId: timezone,
    rawTimezones: timezones,
    nameSchemaId,
    organizationName,
    setLanguageAndTime,
    getPortalTimezones,
    getCurrentCustomSchema,
    helpUrlCommonSettings,
    customNames,
  };
})(
  withCultureNames(
    observer(withTranslation(["Settings", "Common"])(Customization))
  )
);
