import React, { useEffect } from "react";
import { ReactSVG } from "react-svg";
import PropTypes from "prop-types";
import { withRouter } from "react-router";
import Text from "@appserver/components/text";
import Link from "@appserver/components/link";
import Badge from "@appserver/components/badge";
import Box from "@appserver/components/box";
import EmptyScreenContainer from "@appserver/components/empty-screen-container";
import ExternalLinkIcon from "../../../../../../public/images/external.link.react.svg";
import Loaders from "@appserver/common/components/Loaders";
import toastr from "studio/toastr";
import Section from "@appserver/common/components/Section";
import { useTranslation, withTranslation } from "react-i18next";
import styled from "styled-components";
import { isMobile, isIOS } from "react-device-detect";

import { setDocumentTitle } from "../../../helpers/utils";
import { inject } from "mobx-react";
import i18n from "../../../i18n";
import { I18nextProvider } from "react-i18next";

const commonStyles = `
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
    .learn-more-link {
      white-space: nowrap;
    }
  }

  .coming-soon-badge {
    margin-bottom: 26px;
  }

  ${commonStyles}
`;

const StyledDesktopContainer = styled(EmptyScreenContainer)`
  img {
    width: 150px;
    height: 150px;
  }
  span:first-of-type {
    font-size: 24px;
  }
  span {
    font-size: 14px;
    > p {
      font-size: 14px;
      .learn-more-link {
        white-space: nowrap;
      }
    }
  }
  ${commonStyles}

  .view-web-link {
    font-size: 14px;
  }

  .coming-soon-badge > div > p {
    font-size: 13px;
  }
`;

const ExternalLink = ({ label, href }) => (
  <Box className="link-box">
    <ExternalLinkIcon
      color={theme.studio.comingSoon.linkIconColor}
      size={isMobile ? "small" : "medium"}
    />
    <Link
      as="a"
      href={href}
      target="_blank"
      className="view-web-link"
      color={theme.studio.comingSoon.linkColor}
      isBold
      isHovered
    >
      {label}
    </Link>
  </Box>
);

const Body = ({ modules, match, isLoaded, setCurrentProductId, t }) => {
  const { error } = match.params;
  const { pathname, protocol, hostname } = window.location;
  const currentModule = modules.find((m) => m.link === pathname);
  const {
    id,
    title,
    description,
    imageUrl,
    link,
    originUrl,
    helpUrl,
  } = currentModule;
  const url = originUrl ? originUrl : link;
  const webLink = protocol + "//" + hostname + url;
  const appLink = isIOS
    ? id === "2A923037-8B2D-487b-9A22-5AC0918ACF3F"
      ? "message:"
      : id === "32D24CB5-7ECE-4606-9C94-19216BA42086"
      ? "calshow:"
      : false
    : false;

  setDocumentTitle();

  useEffect(() => {
    setCurrentProductId(id);
  }, [id, setCurrentProductId]);

  useEffect(() => error && toastr.error(error), [error]);

  const appButtons = (
    <>
      <Badge
        label={t("Common:ComingSoon")}
        maxWidth="150px"
        borderRadius="2px"
        className="coming-soon-badge"
      />
      <ExternalLink label={t("Common:ViewWeb")} href={webLink} />
      {appLink && (
        <ExternalLink
          label={t("Common:OpenApp", {
            title: title,
          })}
          href={appLink}
        />
      )}
    </>
  );

  const moduleDescription = (
    <Text className="module-info">
      {description}{" "}
      {helpUrl && (
        <Link
          as="a"
          href={helpUrl}
          target="_blank"
          className="learn-more-link"
          color={theme.studio.comingSoon.linkColor}
          isBold
          isHovered
        >
          {t("Common:LearnMore")}...
        </Link>
      )}
    </Text>
  );

  return !isLoaded ? (
    <></>
  ) : isMobile ? (
    <ComingSoonPage>
      <ReactSVG
        className="module-logo-icon"
        loading={() => (
          <Loaders.Rectangle
            width="100"
            height="14"
            backgroundColor={theme.studio.comingSoon.backgroundColor}
            foregroundColor={theme.studio.comingSoon.foregroundColor}
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
        {moduleDescription}
        {appButtons}
      </Box>
    </ComingSoonPage>
  ) : (
    <StyledDesktopContainer
      imageSrc={imageUrl}
      imageAlt={title}
      headerText={title}
      descriptionText={moduleDescription}
      buttons={appButtons}
    />
  );
};

const ComingSoon = (props) => {
  return (
    <Section>
      <Section.SectionBody>
        <Body {...props} />
      </Section.SectionBody>
    </Section>
  );
};

ComingSoon.propTypes = {
  modules: PropTypes.array,
  isLoaded: PropTypes.bool,
};

const ComingSoonWrapper = inject(({ auth }) => ({
  modules: auth.moduleStore.modules,
  isLoaded: auth.isLoaded,
  theme: auth.seetingsStore.theme,
  setCurrentProductId: auth.settingsStore.setCurrentProductId,
}))(withRouter(withTranslation("Common")(ComingSoon)));

export default (props) => (
  <I18nextProvider i18n={i18n}>
    <ComingSoonWrapper {...props} />
  </I18nextProvider>
);
