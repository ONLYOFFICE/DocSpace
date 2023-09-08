import React, { useEffect } from "react";
import { withTranslation } from "react-i18next";
import styled, { css } from "styled-components";
import Text from "@docspace/components/text";
import Box from "@docspace/components/box";
import Link from "@docspace/components/link";
import { inject, observer } from "mobx-react";
import withCultureNames from "@docspace/common/hoc/withCultureNames";

import { Base } from "@docspace/components/themes";
import LoaderCustomizationNavbar from "./sub-components/loaderCustomizationNavbar";
import { StyledArrowRightIcon } from "./Customization/StyledSettings";
import { useNavigate } from "react-router-dom";
import Badge from "@docspace/components/badge";

const StyledComponent = styled.div`
  padding-top: 13px;

  .combo-button-label {
    max-width: 100%;
  }

  .category-item-wrapper {
    padding-bottom: 22px;

    .category-item-heading {
      padding-bottom: 8px;
      svg {
        padding-bottom: 5px;
        ${(props) =>
          props.theme.interfaceDirection === "rtl" &&
          css`
            transform: scaleX(-1);
          `}
      }
      .category-item_paid {
        .paid-badge {
          height: 16px;
        }
        display: flex;
        svg {
          ${(props) =>
            props.theme.interfaceDirection === "rtl" &&
            css`
              transform: scaleX(-1);
            `}
          margin-top: auto;
        }
      }
    }

    .category-item-description {
      color: ${(props) => props.theme.client.settings.common.descriptionColor};
      font-size: 13px;
      font-weight: 400px;
      max-width: 1024px;
      line-height: 20px;
    }

    .inherit-title-link {
      ${(props) =>
        props.theme.interfaceDirection === "rtl"
          ? css`
              margin-left: 4px;
            `
          : css`
              margin-right: 4px;
            `}
      font-size: 16px;
      font-weight: 700;
    }
  }
`;

StyledComponent.defaultProps = { theme: Base };

const CustomizationNavbar = ({
  t,
  isLoaded,
  tReady,
  setIsLoadedCustomizationNavbar,
  isLoadedPage,
  isSettingPaid,
}) => {
  const isLoadedSetting = isLoaded && tReady;
  const navigate = useNavigate();

  useEffect(() => {
    if (isLoadedSetting) setIsLoadedCustomizationNavbar(isLoadedSetting);
  }, [isLoadedSetting]);

  const onClickLink = (e) => {
    e.preventDefault();
    navigate(e.target.pathname);
  };

  return !isLoadedPage ? (
    <LoaderCustomizationNavbar />
  ) : (
    <StyledComponent>
      <div className="category-item-wrapper">
        <div className="category-item-heading">
          <Link
            className="inherit-title-link header"
            onClick={onClickLink}
            truncate={true}
            href={
              "portal-settings/customization/general/language-and-time-zone"
            }
          >
            {t("StudioTimeLanguageSettings")}
          </Link>
          <StyledArrowRightIcon size="small" color="#333333" />
        </div>
        <Text className="category-item-description">
          {t("LanguageAndTimeZoneSettingsNavDescription")}
        </Text>
      </div>
      <div className="category-item-wrapper">
        <div className="category-item-heading">
          <Link
            truncate={true}
            className="inherit-title-link header"
            onClick={onClickLink}
            href={
              "/portal-settings/customization/general/welcome-page-settings"
            }
          >
            {t("CustomTitlesWelcome")}
          </Link>
          <StyledArrowRightIcon size="small" color="#333333" />
        </div>
        <Text className="category-item-description">
          {t("CustomTitlesSettingsNavDescription")}
        </Text>
      </div>

      <div className="category-item-wrapper">
        <div className="category-item-heading">
          <div className="category-item_paid">
            <Link
              truncate={true}
              className="inherit-title-link header"
              onClick={onClickLink}
              href={"/portal-settings/customization/general/dns-settings"}
            >
              {t("DNSSettings")}
            </Link>
            {!isSettingPaid && (
              <Badge
                backgroundColor="#EDC409"
                label={t("Common:Paid")}
                isPaidBadge={true}
                className="paid-badge"
              />
            )}
            <StyledArrowRightIcon size="small" color="#333333" />
          </div>
        </div>
        <Text className="category-item-description">
          {t("DNSSettingsNavDescription")}
        </Text>
      </div>

      <div className="category-item-wrapper">
        <div className="category-item-heading">
          <Link
            truncate={true}
            className="inherit-title-link header"
            onClick={onClickLink}
            href={"/portal-settings/customization/general/portal-renaming"}
          >
            {t("PortalRenaming")}
          </Link>
          <StyledArrowRightIcon size="small" color="#333333" />
        </div>
        <Text className="category-item-description">
          {t("PortalRenamingNavDescription")}
        </Text>
      </div>
    </StyledComponent>
  );
};

export default inject(({ common }) => {
  const { isLoaded, setIsLoadedCustomizationNavbar } = common;
  return {
    isLoaded,
    setIsLoadedCustomizationNavbar,
  };
})(
  withCultureNames(
    observer(withTranslation(["Settings", "Common"])(CustomizationNavbar))
  )
);
