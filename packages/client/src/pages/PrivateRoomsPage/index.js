import DarkGeneralPngUrl from "PUBLIC_DIR/images/dark_general.png";
import React, { useEffect, useState } from "react";
import styled from "styled-components";
import Text from "@docspace/components/text";
import Link from "@docspace/components/link";
import Button from "@docspace/components/button";
import Loader from "@docspace/components/loader";
import Section from "@docspace/common/components/Section";
import { smallTablet, tablet } from "@docspace/components/utils/device";
import { I18nextProvider, Trans, withTranslation } from "react-i18next";
import { useLocation } from "react-router-dom";
import { isMobile } from "react-device-detect";
//import { setDocumentTitle } from "@docspace/client/src/helpers/filesUtils";
import i18n from "./i18n";
import toastr from "@docspace/components/toast/toastr";
import { checkProtocol } from "../../helpers/files-helpers";
import Base from "@docspace/components/themes/base";

const StyledPrivacyPage = styled.div`
  margin-top: ${isMobile ? "80px" : "36px"};

  .privacy-rooms-body {
    display: flex;
    flex-direction: column;
    align-items: center;
    max-width: 770px;
    margin: auto;
    margin-top: 80px;
  }

  .privacy-rooms-text-header {
    margin-bottom: 46px;
  }

  .privacy-rooms-text-dialog {
    margin-top: 32px;
    margin-bottom: 42px;
  }

  .privacy-rooms-text-separator {
    width: 70%;
    margin: 28px 0 42px 0;
    border-bottom: ${(props) => props.theme.filesPrivateRoom.borderBottom};
  }

  .privacy-rooms-install-text {
    text-align: left;

    @media ${smallTablet} {
      text-align: center;
    }
  }

  .privacy-rooms-install {
    display: flex;
    flex-direction: row;

    @media ${smallTablet} {
      flex-direction: column;
    }
  }

  .privacy-rooms-link {
    margin-left: 4px;
    color: ${(props) => props.theme.filesPrivateRoom.linkColor};
  }

  .privacy-rooms-text-description {
    margin-top: 28px;
    color: ${(props) => props.theme.filesPrivateRoom.textColor};
    p {
      margin: 0;
    }
  }

  .privacy-rooms-avatar {
    text-align: left;
    padding-left: 66px;

    @media ${tablet} {
      padding-left: 74px;
    }

    @media ${smallTablet} {
      padding: 0px;
      text-align: center;
    }

    margin: 0px;
  }

  .privacy-rooms-logo {
    text-align: center;
    max-width: 216px;
    max-height: 35px;
  }
`;

StyledPrivacyPage.defaultProps = { theme: Base };

const PrivacyPageComponent = ({ t, tReady }) => {
  //   useEffect(() => {
  //     setDocumentTitle(t("Common:About"));
  //   }, [t]);

  const [isDisabled, setIsDisabled] = useState(false);

  const location = useLocation();

  const onOpenEditorsPopup = async () => {
    setIsDisabled(true);
    checkProtocol(location.search.split("=")[1])
      .then(() => setIsDisabled(false))
      .catch(() => {
        setIsDisabled(false);
        toastr.info(t("PrivacyEditors"));
      });
  };

  return !tReady ? (
    <Loader className="pageLoader" type="rombs" size="40px" />
  ) : (
    <StyledPrivacyPage>
      <div className="privacy-rooms-avatar">
        <Link href="/">
          <img
            className="privacy-rooms-logo"
            src={DarkGeneralPngUrl}
            width="320"
            height="181"
            alt="Logo"
          />
        </Link>
      </div>

      <div className="privacy-rooms-body">
        <Text
          textAlign="center"
          className="privacy-rooms-text-header"
          fontSize="38px"
        >
          {t("PrivacyHeader")}
        </Text>

        <Text as="div" textAlign="center" fontSize="20px" fontWeight={300}>
          <Trans t={t} i18nKey="PrivacyClick" ns="PrivacyPage">
            Click Open <strong>ONLYOFFICE Desktop</strong> in the browser dialog
            to work with the encrypted documents
          </Trans>
          .
        </Text>

        <Text
          textAlign="center"
          className="privacy-rooms-text-dialog"
          fontSize="20px"
          fontWeight={300}
        >
          {t("PrivacyDialog")}.
        </Text>
        <Button
          onClick={onOpenEditorsPopup}
          size="medium"
          primary
          isDisabled={isDisabled}
          label={t("PrivacyButton")}
        />

        <label className="privacy-rooms-text-separator" />

        <div className="privacy-rooms-install">
          <Text
            className="privacy-rooms-install-text"
            fontSize="16px"
            fontWeight={300}
          >
            {t("PrivacyEditors")}?
          </Text>
          <Link
            className="privacy-rooms-link privacy-rooms-install-text"
            fontSize="16px"
            isHovered
            href="https://www.onlyoffice.com/desktop.aspx"
          >
            {t("PrivacyInstall")}
          </Link>
        </div>

        <Text
          as="div"
          fontSize="12px"
          textAlign="center"
          className="privacy-rooms-text-description"
        >
          <p>{t("PrivacyDescriptionEditors")}.</p>
          <p>{t("PrivacyDescriptionConnect")}.</p>
        </Text>
      </div>
    </StyledPrivacyPage>
  );
};

const PrivacyPageWrapper = withTranslation(["PrivacyPage"])(
  PrivacyPageComponent
);

const PrivacyPage = (props) => {
  return (
    <I18nextProvider i18n={i18n}>
      <Section>
        <Section.SectionBody>
          <PrivacyPageWrapper {...props} />
        </Section.SectionBody>
      </Section>
    </I18nextProvider>
  );
};

export default PrivacyPage;
