import React, { useEffect } from "react";
import PropTypes from "prop-types";
import { withRouter } from "react-router";
import { useTranslation } from "react-i18next";
import Text from "@appserver/components/text";
import toastr from "studio/toastr";
import PageLayout from "@appserver/common/components/PageLayout";
import { tryRedirectTo } from "@appserver/common/utils";
import { setDocumentTitle } from "../../../helpers/utils";
import { inject, observer } from "mobx-react";
import { HomeIllustration, ModuleTile, HomeContainer } from "./sub-components";
import Heading from "@appserver/components/heading";

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

const Body = ({ match, isLoaded, availableModules, displayName }) => {
  const { t } = useTranslation(["Home", "translation"]);
  const { error } = match.params;
  setDocumentTitle();

  useEffect(() => error && toastr.error(error), [error]);

  return !isLoaded ? (
    <></>
  ) : (
    <HomeContainer>
      <Tiles
        availableModules={availableModules}
        displayName={displayName}
        t={t}
      />

      <HomeIllustration />

      {!availableModules || !availableModules.length ? (
        <Text className="home-error-text" fontSize="14px" color="#c30">
          {t("translation:NoOneModulesAvailable")}
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
    <PageLayout isHomepage>
      <PageLayout.SectionBody>
        <Body {...rest} />
      </PageLayout.SectionBody>
    </PageLayout>
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
  const { defaultPage } = settingsStore;
  const { displayName } = userStore.user;

  return {
    defaultPage,
    isLoaded,
    availableModules,
    displayName,
  };
})(withRouter(observer(Home)));
