import React from "react";
import Section from "@appserver/common/components/Section";

import SectionHeaderContent from "./Header";
import SectionBodyContent from "./Body";
import { InfoPanelBodyContent } from "../Home/InfoPanel";
import InfoPanelHeaderContent from "../Home/InfoPanel/GalleryHeader";

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
        <InfoPanelHeaderContent />
      </Section.InfoPanelHeader>
      <Section.InfoPanelBody>
        <InfoPanelBodyContent isGallery />
      </Section.InfoPanelBody>
    </Section>
  );
};

export default FormGallery;
