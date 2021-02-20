import React, { useCallback, useEffect } from "react";
import { connect } from "react-redux";
import PropTypes from "prop-types";
import { withRouter } from "react-router";
import { useTranslation } from "react-i18next";
import styled from "styled-components";
import Text from "@appserver/components/src/components/text";
import toastr from "@appserver/common/src/components/Toast/toastr";
import PageLayout from "@appserver/common/src/components/PageLayout";
import history from "@appserver/common/src/history";
import ModuleTile from "./ModuleTile";
import { changeLanguage, tryRedirectTo } from "@appserver/common/src/utils";
import { createI18N } from "../../../helpers/i18n";
import { setDocumentTitle } from "../../../helpers/utils";
import { ReactSVG } from "react-svg";
import { IconButton } from "@appserver/components/src";
import ContextMenuButton from "@appserver/components/src/components/context-menu-button";
import ExpanderDownIcon from "../../../../../../public/images/expander-down.react.svg"
const i18n = createI18N({
  page: "Home",
  localesPath: "pages/Home",
});

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
  const mapped = modules.filter((m) => m.isPrimary === isPrimary);

  return mapped.length > 0 ? (
    <div className="home-modules">
      {mapped.map((module) => (
        <div className="home-module" key={++index}>
          <ModuleTile {...module} onClick={() => history.push(module.link)} />
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
const iconStyle = {
  width: "20px",
  height: "20px",
  minWidth: "20px",
  minHeight: "20px",
};
const Body = ({ modules, match, isLoaded }) => {
  const { t } = useTranslation("translation", { i18n });
  const { error } = match.params;

  setDocumentTitle();

  useEffect(() => error && toastr.error(error), [error]);

  useEffect(() => {
    changeLanguage(i18n);
  }, []);

  console.log("!!")
  return !isLoaded ? (
    <></>
  ) : (
    <HomeContainer>
      <Tiles modules={modules} isPrimary={true} />
      <Tiles modules={modules} isPrimary={false} />

      {/* <IconButton iconName={"static/images/cross.react.svg"} /> */}
      {/* <ContextMenuButton
        className="action-button"
        directionX="right"
        title={t("Actions")}
        //iconName="static/images/cross.react.svg"
        size={17}
        color="#A3A9AE"
        //getData={getContextOptionsGroup}
        isDisabled={false}
      /> */}

      {/* <ExpanderDownIcon color="#A3A9AE" isfill={true} size="scale" style={iconStyle}/> */}
    </HomeContainer>
  );
};

const Home = (props) => {
  const { defaultPage } = props;
  return tryRedirectTo(defaultPage) ? (
    <></>
  ) : (
    <PageLayout>
      <PageLayout.SectionBody>
        <Body {...props} />
      </PageLayout.SectionBody>
    </PageLayout>
  );
};

Home.propTypes = {
  modules: PropTypes.array.isRequired,
  isLoaded: PropTypes.bool,
};

function mapStateToProps(state) {
  const { modules, isLoaded, settings } = state.auth;
  const { defaultPage } = settings;
  return {
    modules,
    isLoaded,
    defaultPage,
  };
}

export default connect(mapStateToProps)(withRouter(Home));
