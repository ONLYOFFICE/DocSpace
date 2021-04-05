import React from "react";
import PageLayout from "../PageLayout";
import Loader from "@appserver/components/loader";

const AppLoader = () => (
  <PageLayout>
    <PageLayout.SectionBody>
      <Loader className="pageLoader" type="rombs" size="40px" />
    </PageLayout.SectionBody>
  </PageLayout>
);

export default AppLoader;
