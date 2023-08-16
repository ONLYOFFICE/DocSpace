import React from "react";
import { inject, observer } from "mobx-react";
import Text from "@docspace/components/text";
import NoUserSelect from "@docspace/components/utils/commonStyles";
import { useTranslation } from "react-i18next";
import styled from "styled-components";
import { ReactSVG } from "react-svg";
import { isMobile } from "react-device-detect";
import { ColorTheme, ThemeType } from "@docspace/common/components/ColorTheme";
import logoPersonalAboutUrl from "PUBLIC_DIR/images/logo_personal_about.svg?url";
import { getLogoFromPath } from "@docspace/common/utils";

const StyledAboutBody = styled.div`
  width: 100%;

  .avatar {
    margin-top: ${!isMobile ? "0px" : "32px"};
    margin-bottom: 16px;
  }

  .row-el {
    display: inline-block;
  }

  .copyright {
    margin-top: 14px;
    margin-bottom: 4px;
    font-weight: 700;
  }
  .no-select {
    ${NoUserSelect}
  }

  .row {
    line-height: 20px;
  }

  .tel-title,
  .address-title {
    display: inline;
  }
  .select-el {
    ${isMobile && `user-select: text`};
  }

  .logo-theme {
    svg {
      g:nth-child(2) {
        path:nth-child(5) {
          fill: ${(props) => props.theme.client.about.logoColor};
        }

        path:nth-child(6) {
          fill: ${(props) => props.theme.client.about.logoColor};
        }
      }
    }
  }

  .logo-docspace-theme {
    height: 24px;
    width: 211px;

    svg {
      path:nth-child(4) {
        fill: ${(props) => props.theme.client.about.logoColor};
      }
    }
  }
`;

const AboutContent = (props) => {
  const {
    personal,
    buildVersionInfo,
    theme,
    companyInfoSettingsData,
    previewData,
    whiteLabelLogoUrls,
  } = props;
  const { t } = useTranslation("About");
  const license = "AGPL-3.0";
  const linkRepo = "https://github.com/ONLYOFFICE/DocSpace";
  const linkDocs = "https://github.com/ONLYOFFICE/DocumentServer";

  const companyName = previewData
    ? previewData.companyName
    : companyInfoSettingsData?.companyName;

  const email = previewData
    ? previewData.email
    : companyInfoSettingsData?.email;

  const phone = previewData
    ? previewData.phone
    : companyInfoSettingsData?.phone;

  const site = previewData ? previewData.site : companyInfoSettingsData?.site;

  const address = previewData
    ? previewData.address
    : companyInfoSettingsData?.address;

  const logo = getLogoFromPath(
    !theme.isBase
      ? whiteLabelLogoUrls[6]?.path.dark
      : whiteLabelLogoUrls[6]?.path.light
  );

  return (
    companyInfoSettingsData && (
      <StyledAboutBody>
        <div className="avatar">
          {personal ? (
            <ReactSVG
              src={logoPersonalAboutUrl}
              className="logo-theme no-select"
            />
          ) : (
            <img
              src={logo}
              alt="Logo"
              className="logo-docspace-theme no-select"
            />
          )}
        </div>

        <div className="row">
          <Text className="row-el" fontSize="13px">
            {t("DocumentManagement")}:
          </Text>
          <ColorTheme
            {...props}
            tag="a"
            themeId={ThemeType.Link}
            className="row-el"
            fontSize="13px"
            fontWeight="600"
            href={linkRepo}
            target="_blank"
            enableUserSelect
          >
            &nbsp;ONLYOFFICE DocSpace&nbsp;
          </ColorTheme>

          <Text
            className="row-el select-el"
            fontSize="13px"
            fontWeight="600"
            title={`${BUILD_AT}`}
          >
            v.{buildVersionInfo.docspace}
          </Text>
        </div>

        <div className="row">
          <Text className="row-el" fontSize="13px">
            {t("OnlineEditors")}:
          </Text>
          <ColorTheme
            {...props}
            tag="a"
            themeId={ThemeType.Link}
            className="row-el"
            fontSize="13px"
            fontWeight="600"
            href={linkDocs}
            target="_blank"
            enableUserSelect
          >
            &nbsp;ONLYOFFICE Docs&nbsp;
          </ColorTheme>
          <Text className="row-el select-el" fontSize="13px" fontWeight="600">
            v.{buildVersionInfo.documentServer}
          </Text>
        </div>

        <div className="row">
          <Text className="row-el" fontSize="13px">
            {t("SoftwareLicense")}:{" "}
          </Text>
          <Text className="row-el" fontSize="13px" fontWeight="600">
            &nbsp;{license}
          </Text>
        </div>

        <Text className="copyright" fontSize="14px" fontWeight="600">
          © {companyName}
        </Text>

        <div className="row">
          <Text className="address-title" fontSize="13px">
            {t("Common:Address")}:{" "}
          </Text>
          <Text className="address-title select-el" fontSize="13px">
            {address}
          </Text>
        </div>

        <div className="row">
          <Text className="tel-title" fontSize="13px">
            {t("Common:Phone")}:{" "}
          </Text>
          <Text className="tel-title select-el" fontSize="13px">
            {phone}
          </Text>
        </div>

        <div className="row">
          <Text className="row-el" fontSize="13px">
            {t("AboutCompanyEmailTitle")}:
          </Text>

          <ColorTheme
            {...props}
            tag="a"
            themeId={ThemeType.Link}
            className="row-el"
            fontSize="13px"
            fontWeight="600"
            href={`mailto:${companyInfoSettingsData.email}`}
            enableUserSelect
          >
            &nbsp;{email}
          </ColorTheme>
        </div>

        <div className="row">
          <Text className="row-el" fontSize="13px">
            {t("Site")}:
          </Text>

          <ColorTheme
            {...props}
            tag="a"
            themeId={ThemeType.Link}
            className="row-el"
            fontSize="13px"
            fontWeight="600"
            target="_blank"
            href={site}
            enableUserSelect
          >
            &nbsp;{site.replace(/^https?\:\/\//i, "")}
          </ColorTheme>
        </div>
      </StyledAboutBody>
    )
  );
};

export default inject(({ auth }) => {
  const { settingsStore } = auth;

  const { theme, companyInfoSettingsData, whiteLabelLogoUrls } = settingsStore;

  return {
    theme,
    companyInfoSettingsData,
    whiteLabelLogoUrls,
  };
})(observer(AboutContent));
