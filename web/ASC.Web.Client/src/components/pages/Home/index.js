import React, { useEffect } from "react";
import PropTypes from "prop-types";
import { withRouter } from "react-router";
import { useTranslation } from "react-i18next";
import Text from "@appserver/components/text";
import toastr from "studio/toastr";
import Section from "@appserver/common/components/Section";
import tryRedirectTo from "@appserver/common/utils/tryRedirectTo";
import { setDocumentTitle } from "../../../helpers/utils";
import { inject, observer } from "mobx-react";
import { HomeIllustration, ModuleTile, HomeContainer } from "./sub-components";
import Heading from "@appserver/components/heading";
import AppLoader from "@appserver/common/components/AppLoader";

const Tiles = ({ availableModules, displayName, t }) => {
  let index = 0;

  const getGreeting = (displayName) => {
    const name = displayName.trim();
    const time = new Date().getHours();

    if (time >= 5 && time <= 11) return t("GoodMorning", { displayName: name });
    if (time >= 12 && time <= 16)
      return t("GoodAfternoon", { displayName: name });
    return t("GoodEvening", { displayName: name });
  };

  const greetingMessage = getGreeting(displayName);

  const modules = availableModules.filter(
    (module) => module.separator !== true && module.id !== "settings"
  );

  return modules.length > 0 ? (
    <div className="home-modules-container">
      <Heading className="greeting">{greetingMessage}</Heading>

      <div className="home-modules">
        {modules.map((m) => (
          <div className="home-module" key={`${++index}-${m.appName}`}>
            <ModuleTile {...m} />
          </div>
        ))}
      </div>
    </div>
  ) : (
    <></>
  );
};

Tiles.propTypes = {
  availableModules: PropTypes.array.isRequired,
  displayName: PropTypes.string,
  t: PropTypes.func,
};

const Body = ({
  match,
  isLoaded,
  availableModules,
  displayName,
  snackbarExist,
  theme,
}) => {
  const { t, ready } = useTranslation("Home");
  const { error } = match.params;
  setDocumentTitle();

  useEffect(() => error && toastr.error(error), [error]);

  return !isLoaded || !ready ? (
    <AppLoader />
  ) : (
    <HomeContainer snackbarExist={snackbarExist}>
      <Tiles
        availableModules={availableModules}
        displayName={displayName}
        t={t}
      />

      <HomeIllustration />

      {!availableModules || !availableModules.length ? (
        <Text
          className="home-error-text"
          fontSize="14px"
          color={theme.studio.home.textColorError}
        >
          {t("NoOneModulesAvailable")}
        </Text>
      ) : null}
    </HomeContainer>
  );
};

Body.propTypes = {
  availableModules: PropTypes.array.isRequired,
  isLoaded: PropTypes.bool,
  match: PropTypes.object,
  displayName: PropTypes.string,
};

const Home = ({ defaultPage, ...rest }) => {
  return tryRedirectTo(defaultPage) ? (
    <></>
  ) : (
    <Section isHomepage>
      <Section.SectionBody>
        <Body {...rest} />
      </Section.SectionBody>
    </Section>
  );
};

Home.propTypes = {
  availableModules: PropTypes.array.isRequired,
  isLoaded: PropTypes.bool,
  defaultPage: PropTypes.string,
  displayName: PropTypes.string,
};

export default inject(({ auth }) => {
  const { isLoaded, settingsStore, availableModules, userStore } = auth;
  const { defaultPage, snackbarExist, theme } = settingsStore;
  const { displayName } = userStore.user;

  return {
    theme,
    defaultPage,
    isLoaded,
    availableModules,
    displayName,
    snackbarExist,
  };
})(withRouter(observer(Home)));
