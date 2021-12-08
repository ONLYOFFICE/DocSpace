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

const Tiles = ({ availableModules, greetingSettings }) => {
  let index = 0;

  const modules = availableModules.filter(
    (module) => module.separator !== true && module.id !== "settings"
  );

  return modules.length > 0 ? (
    <div className="home-modules-container">
      <Heading>{greetingSettings}</Heading>

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
  greetingSettings: PropTypes.string,
};

const Body = ({ match, isLoaded, availableModules, greetingSettings }) => {
  const { t } = useTranslation();
  const { error } = match.params;
  setDocumentTitle();

  useEffect(() => error && toastr.error(error), [error]);

  return !isLoaded ? (
    <></>
  ) : (
    <HomeContainer>
      <HomeIllustration />

      <Tiles
        availableModules={availableModules}
        greetingSettings={greetingSettings}
      />

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
  greetingSettings: PropTypes.string,
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
  greetingSettings: PropTypes.string,
};

export default inject(({ auth }) => {
  const { isLoaded, settingsStore, availableModules } = auth;
  const { defaultPage, greetingSettings } = settingsStore;
  return {
    defaultPage,
    isLoaded,
    availableModules,
    greetingSettings,
  };
})(withRouter(observer(Home)));
