import React, { useEffect } from "react";
import Section from "@docspace/common/components/Section";
import { observer, inject } from "mobx-react";

import SectionHeaderContent from "./Header";
import SectionBodyContent from "./Body";
import { InfoPanelBodyContent } from "../Home/InfoPanel";
import InfoPanelHeaderContent from "../Home/InfoPanel/Header";

const FormGallery = ({ getOforms, setOformFiles }) => {
  useEffect(() => {
    getOforms();

    return () => {
      setOformFiles(null);
    };
  }, [getOforms, setOformFiles]);

  return (
    <Section
      // withBodyScroll
      // withBodyAutoFocus={!isMobile}
      withPaging={false}
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

export default inject(({ oformsStore }) => {
  const { getOforms, setOformFiles } = oformsStore;

  return {
    getOforms,
    setOformFiles,
  };
})(observer(FormGallery));
