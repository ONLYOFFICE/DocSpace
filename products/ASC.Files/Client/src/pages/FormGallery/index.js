import React from "react";
import Section from "@appserver/common/components/Section";

import SectionHeaderContent from "./Header";
import SectionBodyContent from "./Body";

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
    </Section>
  );
};

export default FormGallery;
