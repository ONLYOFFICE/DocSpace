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

const Tiles = ({ availableModules, username, t }) => {
  let index = 0;
  const { firstName, lastName } = username;

  const getGreeting = () => {
    const time = new Date().getHours();

    if (time >= 5 && time <= 11) return t("GoodMorning");
    if (time >= 12 && time <= 16) return t("GoodAfternoon");
    else return t("GoodEvening");
  };

  const greetingMessage = `${getGreeting()}, ${firstName} ${lastName}!`;

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
  username: PropTypes.object,
  t: PropTypes.func,
};

const Body = ({ match, isLoaded, availableModules, username }) => {
  const { t } = useTranslation(["Home", "Common"]);
  const { error } = match.params;
  setDocumentTitle();

  useEffect(() => error && toastr.error(error), [error]);

  return !isLoaded ? (
    <></>
  ) : (
    <HomeContainer>
      <Tiles availableModules={availableModules} username={username} t={t} />

      <HomeIllustration />

      {!availableModules || !availableModules.length ? (
        <Text className="home-error-text" fontSize="14px" color="#c30">
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
  username: PropTypes.object,
};

const Home = ({ defaultPage, ...rest }) => {
  return tryRedirectTo(defaultPage) ? (
    <></>
  ) : (
    <PageLayout>
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
  username: PropTypes.object,
};

export default inject(({ auth }) => {
  console.log(auth);
  const { isLoaded, settingsStore, availableModules, userStore } = auth;
  const { defaultPage } = settingsStore;
  const { firstName, lastName } = userStore.user;
  const username = {
    firstName,
    lastName,
  };
  return {
    defaultPage,
    isLoaded,
    availableModules,
    username,
  };
})(withRouter(observer(Home)));
