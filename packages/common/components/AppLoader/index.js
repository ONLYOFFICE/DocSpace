import React from "react";
import Section from "../Section";
import Loader from "@docspace/components/loader";

const AppLoader = () => (
  <Section>
    <Section.SectionBody>
      <Loader className="pageLoader" type="rombs" size="40px" />
    </Section.SectionBody>
  </Section>
);

export default AppLoader;
