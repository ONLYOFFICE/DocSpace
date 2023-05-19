import React from "react";
import { inject, observer } from "mobx-react";
import { ReactSVG } from "react-svg";
import { useTranslation } from "react-i18next";

import Text from "@docspace/components/text";

import LifetimeLicenseReactSvgUrl from "PUBLIC_DIR/images/lifetime_license.react.svg?url";
import TechSupportReactSvgUrl from "PUBLIC_DIR/images/tech_support.react.svg?url";
import MobileEditingReactSvgUrl from "PUBLIC_DIR/images/mobile_editing.react.svg?url";
import ScalabilityReactSvgUrl from "PUBLIC_DIR/images/scalability.react.svg?url";

import { StyledBenefitsBody } from "./StyledComponent";

const BenefitsContainer = ({ theme, isTrial, isEnterprise }) => {
  const { t } = useTranslation("PaymentsEnterprise");

  const title = isEnterprise
    ? t("ActivateToProBannerHeaderTrial")
    : t("UpgradeToProBannerHeader");

  const features = () => {
    const techSupport = {
      imag: TechSupportReactSvgUrl,
      title: t("UpgradeToProBannerItemSupportHeader"),
      description: t("UpgradeToProBannerItemSupportDescr"),
    };

    const mobileEditing = {
      imag: MobileEditingReactSvgUrl,
      title: t("UpgradeToProBannerItemMobileHeader"),
      description: t("UpgradeToProBannerItemMobileDescr"),
    };

    const lifetimeLicense = {
      imag: LifetimeLicenseReactSvgUrl,
      title: t("UpgradeToProBannerItemLicenseHeader"),
      description: t("UpgradeToProBannerItemLicenseDescr"),
    };

    const scalabilityClustering = {
      imag: ScalabilityReactSvgUrl,
      title: t("UpgradeToProBannerItemScalabilityHeader"),
      description: t("UpgradeToProBannerItemScalabilityDescr"),
    };

    const featuresArray = [];

    if (isEnterprise) {
      isTrial
        ? featuresArray.push(
            scalabilityClustering,
            mobileEditing,
            lifetimeLicense,
            techSupport
          )
        : featuresArray.push(lifetimeLicense, techSupport);
    } else {
      featuresArray.push(mobileEditing, techSupport);
    }

    return featuresArray.map((item, index) => {
      return (
        <div className="payments-benefits" key={index}>
          <ReactSVG src={item.imag} className="benefits-svg" />
          <div className="benefits-description">
            <Text fontWeight={600}>{item.title}</Text>
            <Text>{item.description}</Text>
          </div>
        </div>
      );
    });
  };

  return (
    <StyledBenefitsBody className="benefits-container" theme={theme}>
      <Text fontSize={"16px"} fontWeight={600} className="benefits-title">
        {title}
      </Text>
      {features()}
    </StyledBenefitsBody>
  );
};

export default inject(({ auth }) => {
  const {
    paymentQuotasStore,
    settingsStore,
    currentQuotaStore,
    isEnterprise,
  } = auth;
  const { theme } = settingsStore;
  const { portalPaymentQuotasFeatures } = paymentQuotasStore;

  const { isTrial } = currentQuotaStore;
  return {
    theme,
    features: portalPaymentQuotasFeatures,
    isTrial,
    isEnterprise,
  };
})(observer(BenefitsContainer));
