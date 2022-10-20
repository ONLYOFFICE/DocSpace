import React from "react";
import { inject, observer } from "mobx-react";
import Text from "@docspace/components/text";
import Link from "@docspace/components/link";
import NoUserSelect from "@docspace/components/utils/commonStyles";
import { useTranslation } from "react-i18next";
import styled from "styled-components";
import { ReactSVG } from "react-svg";
import { isMobile } from "react-device-detect";
import { ColorTheme, ThemeType } from "@docspace/common/components/ColorTheme";

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
    docSpaceLogo,
  } = props;
  const { t } = useTranslation("About");
  const license = "AGPL-3.0";
  const linkAppServer = "https://github.com/ONLYOFFICE/DocSpace";
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

  return (
    companyInfoSettingsData && (
      <StyledAboutBody>
        <div className="avatar">
          {personal ? (
            <ReactSVG
              src="/images/logo_personal_about.svg"
              className="logo-theme no-select"
            />
          ) : (
            <ReactSVG
              src={docSpaceLogo}
              alt="Logo"
              className="logo-docspace-theme no-select"
            />
          )}
        </div>

        <div className="row">
          <Text className="row-el" fontSize="13px" noSelect>
            {t("DocumentManagement")}:
          </Text>
          <ColorTheme
            {...props}
            tag="a"
            themeId={ThemeType.Link}
            className="row-el"
            fontSize="13px"
            fontWeight="600"
            href={linkAppServer}
            target="_blank"
          >
            &nbsp;ONLYOFFICE DocSpace&nbsp;
          </ColorTheme>

          <Text className="row-el select-el" fontSize="13px" fontWeight="600">
            v.{buildVersionInfo.appServer}
          </Text>
        </div>

        <div className="row">
          <Text className="row-el" fontSize="13px" noSelect>
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
          >
            &nbsp;ONLYOFFICE Docs&nbsp;
          </ColorTheme>
          <Text className="row-el select-el" fontSize="13px" fontWeight="600">
            v.{buildVersionInfo.documentServer}
          </Text>
        </div>

        <div className="row">
          <Text className="row-el" fontSize="13px" noSelect>
            {t("SoftwareLicense")}:{" "}
          </Text>
          <Text className="row-el" fontSize="13px" fontWeight="600" noSelect>
            &nbsp;{license}
          </Text>
        </div>

        <Text className="copyright" fontSize="14px" fontWeight="600" noSelect>
          © {companyName}
        </Text>

        <div className="row">
          <Text className="address-title" fontSize="13px" noSelect>
            {t("AboutCompanyAddressTitle")}:{" "}
          </Text>
          <Text className="address-title select-el" fontSize="13px">
            {address}
          </Text>
        </div>

        <div className="row">
          <Text className="tel-title" fontSize="13px" noSelect>
            {t("Common:Phone")}:{" "}
          </Text>
          <Text className="tel-title select-el" fontSize="13px">
            {phone}
          </Text>
        </div>

        <div className="row">
          <Text className="row-el" fontSize="13px" noSelect>
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
          >
            &nbsp;{email}
          </ColorTheme>
        </div>

        <div className="row">
          <Text className="row-el" fontSize="13px" noSelect>
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

  const { theme, companyInfoSettingsData, docSpaceLogo } = settingsStore;

  return { theme, companyInfoSettingsData, docSpaceLogo };
})(observer(AboutContent));
