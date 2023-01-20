import Section from "@docspace/common/components/Section";
import React from "react";
import SectionBodyContent from "./Section/Body/index";
import SectionHeaderContent from "./Section/Header/index";
import { useTranslation } from "react-i18next";
const NotificationComponent = (props) => {
  const { history } = props;
  const { t } = useTranslation("Notifications");
  return (
    <Section withBodyAutoFocus viewAs="profile">
      <Section.SectionHeader>
        <SectionHeaderContent history={history} t={t} />
      </Section.SectionHeader>

      <Section.SectionBody>
        <SectionBodyContent t={t} />
      </Section.SectionBody>
    </Section>
  );
};

export default NotificationComponent;
