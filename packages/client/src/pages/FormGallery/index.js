import React from "react";
import Section from "@docspace/common/components/Section";

import SectionHeaderContent from "./Header";
import SectionBodyContent from "./Body";
import { InfoPanelBodyContent } from "../Home/InfoPanel";
import InfoPanelHeaderContent from "../Home/InfoPanel/Header";

const FormGallery = () => {
  return (
    <Section
    // withBodyScroll
    // withBodyAutoFocus={!isMobile}
    >
      <Section.SectionHeader>
        <SectionHeaderContent />
      </Section.SectionHeader>
      <Section.SectionBody>
        <SectionBodyContent />
      </Section.SectionBody>
      <Section.InfoPanelHeader>
        <InfoPanelHeaderContent isGallery />
      </Section.InfoPanelHeader>
      <Section.InfoPanelBody>
        <InfoPanelBodyContent isGallery />
      </Section.InfoPanelBody>
    </Section>
  );
};

export default FormGallery;
