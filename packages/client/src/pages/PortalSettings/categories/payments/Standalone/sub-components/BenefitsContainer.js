import React from "react";
import { inject, observer } from "mobx-react";
import { ReactSVG } from "react-svg";

import Text from "@docspace/components/text";

import LifetimeLicenseReactSvgUrl from "PUBLIC_DIR/images/lifetime_license.react.svg?url";
import TechSupportReactSvgUrl from "PUBLIC_DIR/images/tech_support.react.svg?url";
import MobileEditingReactSvgUrl from "PUBLIC_DIR/images/mobile_editing.react.svg?url";
import ScalabilityReactSvgUrl from "PUBLIC_DIR/images/scalability.react.svg?url";

import { StyledBenefitsBody } from "../StyledComponent";

const BenefitsContainer = ({
  t,
  theme,
  isTrial,
  isEnterprise,
  isCommunity,
}) => {
  const title =
    isEnterprise || isTrial ? t("FullEnterpriseVersion") : t("FreeProFeatures");

  const features = () => {
    const techSupport = {
      imag: TechSupportReactSvgUrl,
      title: "Tech support",
      description: "Get quick professional help for all the issues you face",
    };

    const mobileEditing = {
      imag: MobileEditingReactSvgUrl,
      title: "Mobile editing",
      description: "Edit docs, sheets, and slides in mobile browsers",
    };

    const lifetimeLicense = {
      imag: LifetimeLicenseReactSvgUrl,
      title: "Lifetime license",
      description:
        "+ 1-year subscription for functionality and security updates",
    };

    const scalabilityClustering = {
      imag: ScalabilityReactSvgUrl,
      title: "Scalability and clustering",
      description:
        "Comfortably edit and collaborate on docs no matter what size your team is",
    };

    const featuresArray = [];

    isEnterprise
      ? featuresArray.push(lifetimeLicense, techSupport)
      : isCommunity
      ? featuresArray.push(mobileEditing, techSupport)
      : featuresArray.push(
          scalabilityClustering,
          mobileEditing,
          lifetimeLicense,
          techSupport
        );

    return featuresArray.map((item, index) => {
      return (
        <div className="payments-benefits" key={index}>
          <ReactSVG src={item.imag} className="benefits-svg" />
          <div className="benefits-description">
            <Text isBold>{item.title}</Text>
            <Text>{item.description}</Text>
          </div>
        </div>
      );
    });
  };

  return (
    <StyledBenefitsBody className="benefits-container" theme={theme}>
      <Text fontSize={"16px"} isBold className="benefits-title">
        {title}
      </Text>
      {features()}
    </StyledBenefitsBody>
  );
};

export default inject(({ auth }) => {
  const { paymentQuotasStore, settingsStore } = auth;
  const { theme } = settingsStore;
  const { portalPaymentQuotasFeatures } = paymentQuotasStore;

  const isTrial = false,
    isEnterprise = true,
    isCommunity = false;
  return {
    theme,
    features: portalPaymentQuotasFeatures,
    isTrial,
    isEnterprise,
    isCommunity,
  };
})(observer(BenefitsContainer));
