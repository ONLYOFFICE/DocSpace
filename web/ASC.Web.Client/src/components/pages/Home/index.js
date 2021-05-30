import React, { useEffect } from "react";
import PropTypes from "prop-types";
import { withRouter } from "react-router";
import { useTranslation } from "react-i18next";
import styled from "styled-components";
import Text from "@appserver/components/text";
import toastr from "studio/toastr";
import PageLayout from "@appserver/common/components/PageLayout";
import history from "@appserver/common/history";
import ModuleTile from "./ModuleTile";
import { tryRedirectTo } from "@appserver/common/utils";
import { setDocumentTitle } from "../../../helpers/utils";
import { inject, observer } from "mobx-react";
import config from "../../../../package.json";

const HomeContainer = styled.div`
  padding: 62px 15px 0 15px;
  margin: 0 auto;
  max-width: 1140px;
  width: 100%;
  box-sizing: border-box;
  /*justify-content: center;*/

  .home-modules {
    display: flex;
    flex-wrap: wrap;
    margin: 0 -15px;

    .home-module {
      flex-basis: 0;
      flex-grow: 1;
      max-width: 100%;
    }
  }

  .home-error-text {
    margin-top: 23px;
    padding: 0 30px;
    @media (min-width: 768px) {
      margin-left: 25%;
      flex: 0 0 50%;
      max-width: 50%;
    }
    @media (min-width: 576px) {
      flex: 0 0 100%;
      max-width: 100%;
    }
  }
`;

const Tiles = ({ modules, isPrimary }) => {
  let index = 0;
  const mapped = modules.filter(
    (m) => m.isPrimary === isPrimary && m.isolateMode !== true
  );

  //console.log("Tiles", mapped, isPrimary);

  return mapped.length > 0 ? (
    <div className="home-modules">
      {mapped.map((m) => (
        <div className="home-module" key={++index}>
          <ModuleTile {...m} onClick={() => history.push(m.link)} />
        </div>
      ))}
    </div>
  ) : (
    <></>
  );
};

Tiles.propTypes = {
  modules: PropTypes.array.isRequired,
  isPrimary: PropTypes.bool.isRequired,
};

const Body = ({ modules, match, isLoaded }) => {
  const { t } = useTranslation();
  const { error } = match.params;
  setDocumentTitle();

  useEffect(() => error && toastr.error(error), [error]);

  return !isLoaded ? (
    <></>
  ) : (
    <HomeContainer>
      <Tiles modules={modules} isPrimary={true} />
      <Tiles modules={modules} isPrimary={false} />

      {!modules || !modules.length ? (
        <Text className="home-error-text" fontSize="14px" color="#c30">
          {t("NoOneModulesAvailable")}
        </Text>
      ) : null}
    </HomeContainer>
  );
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
  modules: PropTypes.array.isRequired,
  isLoaded: PropTypes.bool,
  defaultPage: PropTypes.string,
};

export default inject(({ auth }) => {
  const { isLoaded, settingsStore, moduleStore } = auth;
  const { defaultPage } = settingsStore;
  const { modules } = moduleStore;
  return {
    defaultPage,
    modules,
    isLoaded,
  };
})(withRouter(observer(Home)));
