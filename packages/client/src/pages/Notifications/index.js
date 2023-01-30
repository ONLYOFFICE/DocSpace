import Section from "@docspace/common/components/Section";
import React from "react";
import SectionBodyContent from "./Section/Body/index";
import SectionHeaderContent from "./Section/Header/index";
import { useTranslation } from "react-i18next";
const NotificationComponent = (props) => {
  const { history } = props;
  const { t, ready } = useTranslation("Notifications");
  return (
    <Section>
      <Section.SectionHeader>
        <SectionHeaderContent history={history} t={t} />
      </Section.SectionHeader>

      <Section.SectionBody>
        <SectionBodyContent t={t} ready={ready} />
      </Section.SectionBody>
    </Section>
  );
};

export default NotificationComponent;
