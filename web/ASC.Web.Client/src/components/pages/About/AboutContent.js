import React from "react";
import { inject, observer } from "mobx-react";
import Text from "@appserver/components/text";
import Link from "@appserver/components/link";
import NoUserSelect from "@appserver/components/utils/commonStyles";
import { useTranslation } from "react-i18next";
import styled from "styled-components";
import { ReactSVG } from "react-svg";
import { isDesktop, isMobile } from "react-device-detect";

const StyledAboutBody = styled.div`
  width: 100%;

  .avatar {
    margin-top: ${isDesktop ? "0px" : "32px"};
    margin-bottom: 16px;
  }

  .row-el {
    display: inline-block;
  }

  .copyright {
    margin-top: 16px;
  }
  .no-select {
    ${NoUserSelect}
  }

  .tel-title,
  .address-title {
    display: inline;
  }
  .select-el {
    ${isMobile && `user-select: text`};
  }

  .logo-theme {
    #svg_4-4 {
      fill: ${(props) => props.theme.studio.about.logoColor};
    }

    #svg_5-5 {
      fill: ${(props) => props.theme.studio.about.logoColor};
    }
  }
`;

const AboutContent = ({ personal, buildVersionInfo, theme }) => {
  const { t } = useTranslation("About");
  const license = "AGPL-3.0";
  const linkAppServer = "https://github.com/ONLYOFFICE/AppServer";
  const linkDocs = "https://github.com/ONLYOFFICE/DocumentServer";
  const phone = "+371 660-16425";
  const email = "support@onlyoffice.com";
  const address =
    "20A-12 Ernesta Birznieka-Upisha street, Riga, Latvia, EU, LV-1050";

  return (
    <StyledAboutBody>
      <div className="avatar">
        {personal ? (
          <ReactSVG
            src="/images/logo_personal_about.svg"
            className="logo-theme no-select"
          />
        ) : (
          <img
            src={
              theme.isBase
                ? "/images/dark_general.png"
                : "/images/white_general.png"
            }
            alt="Logo"
            className="no-select"
          />
        )}
      </div>

      <div className="row">
        <Text className="row-el" fontSize="13px" noSelect>
          {t("DocumentManagement")}:
        </Text>
        <Link
          className="row-el"
          color={theme.studio.about.linkColor}
          fontSize="13px"
          fontWeight="600"
          href={linkAppServer}
          target="_blank"
        >
          &nbsp;ONLYOFFICE App Server&nbsp;
        </Link>
        <Text className="row-el select-el" fontSize="13px" fontWeight="600">
          v.{buildVersionInfo.appServer}
        </Text>
      </div>

      <div className="row">
        <Text className="row-el" fontSize="13px" noSelect>
          {t("OnlineEditors")}:
        </Text>
        <Link
          className="row-el"
          color={theme.studio.about.linkColor}
          fontSize="13px"
          fontWeight="600"
          href={linkDocs}
          target="_blank"
        >
          &nbsp;ONLYOFFICE Docs&nbsp;
        </Link>
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
        © Ascensio System SIA
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
          {t("AboutCompanyTelTitle")}:{" "}
        </Text>
        <Text className="tel-title select-el" fontSize="13px">
          {phone}
        </Text>
      </div>

      <div className="row">
        <Text className="row-el" fontSize="13px" noSelect>
          {t("AboutCompanyEmailTitle")}:
        </Text>
        <Link
          className="row-el"
          color={theme.studio.about.linkColor}
          fontSize="13px"
          fontWeight="600"
          href={`mailto:${email}`}
        >
          &nbsp;{email}
        </Link>
      </div>
    </StyledAboutBody>
  );
};

export default inject(({ auth }) => {
  return { theme: auth.settingsStore.theme };
})(observer(AboutContent));
