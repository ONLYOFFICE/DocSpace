import React, { useEffect } from "react";
import { ReactSVG } from "react-svg";
import { connect } from "react-redux";
import PropTypes from "prop-types";
import { withRouter } from "react-router";
import { Text, Link, Icons, Badge, Box } from "asc-web-components";
import { toastr, PageLayout, utils, store, Loaders } from "asc-web-common";
import { useTranslation } from "react-i18next";
import styled from "styled-components";
import { isMobile, isIOS, isAndroid } from "react-device-detect";

import { createI18N } from "../../../helpers/i18n";
import { setDocumentTitle } from "../../../helpers/utils";

const { getModules, getIsLoaded } = store.auth.selectors;
const { setCurrentProductId } = store.auth.actions;

const i18n = createI18N({
  page: "ComingSoon",
  localesPath: "pages/ComingSoon",
});

const { changeLanguage } = utils;

const ComingSoonPage = styled.div`
  padding: ${isMobile ? "62px 0 0 0" : "0"};
  width: 336px;
  margin: 0 auto;

  .module-logo-icon {
    float: left;
    margin-top: 8px;
    margin-right: 16px;
  }

  .module-title {
    margin-top: 14px;
    margin-bottom: 14px;
  }

  .module-info {
    margin-bottom: 18px;
  }

  .coming-soon-badge {
    margin-bottom: 26px;
  }

  .link-box {
    margin: 8px 0;
    .view-web-link {
      margin: 8px;
      :focus {
        outline: 0;
      }
    }
  }
`;

const ExternalLink = ({ label, href }) => (
  <Box className="link-box">
    <Icons.ExternalLinkIcon color="#333333" size="small" />
    <Link
      as="a"
      href={href}
      target="_blank"
      className="view-web-link"
      color="#555F65"
      isBold
      isHovered
    >
      {label}
    </Link>
  </Box>
);

const Body = ({ modules, match, isLoaded, setCurrentProductId }) => {
  const { t } = useTranslation("translation", { i18n });
  const { error } = match.params;
  const pageLink = window.location.pathname;
  const currentModule = modules.find((m) => m.link === pageLink);
  const { id, title, description, imageUrl, link } = currentModule;
  const appLink =
    id === "2A923037-8B2D-487b-9A22-5AC0918ACF3F"
      ? "mailto:"
      : id === "32D24CB5-7ECE-4606-9C94-19216BA42086"
      ? isIOS
        ? "webcal:"
        : isAndroid
        ? "content://com.android.calendar/time/"
        : false
      : false;

  setDocumentTitle();

  useEffect(() => {
    setCurrentProductId(id);
  }, [id, setCurrentProductId]);

  useEffect(() => error && toastr.error(error), [error]);

  useEffect(() => {
    changeLanguage(i18n);
  }, []);

  return !isLoaded ? (
    <></>
  ) : (
    <ComingSoonPage>
      <ReactSVG
        className="module-logo-icon"
        loading={() => (
          <Loaders.Rectangle
            width="100"
            height="14"
            backgroundColor="#fff"
            foregroundColor="#fff"
            backgroundOpacity={0.25}
            foregroundOpacity={0.2}
          />
        )}
        src={imageUrl}
      />
      <Box displayProp="flex" flexDirection="column" widthProp="220px">
        <Text fontWeight="600" fontSize="19px" className="module-title">
          {title}
        </Text>
        <Text className="module-info">{description}</Text>
        <Badge
          label={t("ComingSoon")}
          maxWidth="150px"
          borderRadius="2px"
          className="coming-soon-badge"
        />
        <ExternalLink label={t("ViewWeb")} href={link} />
        {appLink && (
          <ExternalLink
            label={t("OpenApp", {
              title: title,
            })}
            href={appLink}
          />
        )}
      </Box>
    </ComingSoonPage>
  );
};

const ComingSoon = (props) => {
  return (
    <PageLayout>
      <PageLayout.SectionBody>
        <Body {...props} />
      </PageLayout.SectionBody>
    </PageLayout>
  );
};

ComingSoon.propTypes = {
  modules: PropTypes.array,
  isLoaded: PropTypes.bool,
};

const mapStateToProps = (state) => {
  return {
    modules: getModules(state),
    isLoaded: getIsLoaded(state),
  };
};

export default connect(mapStateToProps, { setCurrentProductId })(
  withRouter(ComingSoon)
);
