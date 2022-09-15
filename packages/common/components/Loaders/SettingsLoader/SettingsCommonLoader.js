import React from "react";
import { LoaderStyle } from "../../../constants";
import RectangleLoader from "../RectangleLoader";
import Box from "@docspace/components/box";

const speed = 2;
const heightText = "20px";
const heightRadio = "16px";
const sectionGap = "16px";
const sectionsGap = "32px";

const Setting = ({ width = "280px" }) => (
  <Box
    displayProp="grid"
    style={{
      gridGap: "8px",
      gridTemplateColumns: `28px ${width}`,
      alignItems: "center",
    }}
  >
    <RectangleLoader
      height={heightRadio}
      backgroundColor={LoaderStyle.backgroundColor}
      foregroundColor={LoaderStyle.foregroundColor}
      backgroundOpacity={LoaderStyle.backgroundOpacity}
      foregroundOpacity={LoaderStyle.foregroundOpacity}
      speed={speed}
      animate={true}
    />
    <RectangleLoader
      height={heightText}
      backgroundColor={LoaderStyle.backgroundColor}
      foregroundColor={LoaderStyle.foregroundColor}
      backgroundOpacity={LoaderStyle.backgroundOpacity}
      foregroundOpacity={LoaderStyle.foregroundOpacity}
      speed={speed}
      animate={true}
    />
  </Box>
);

const SectionTitle = ({ height = 16, width = 62 }) => (
  <RectangleLoader
    height={height}
    width={width}
    backgroundColor={LoaderStyle.backgroundColor}
    foregroundColor={LoaderStyle.foregroundColor}
    backgroundOpacity={LoaderStyle.backgroundOpacity}
    foregroundOpacity={LoaderStyle.foregroundOpacity}
    speed={speed}
    animate={true}
  />
);

const SettingsSection = ({ width1, width2, withTitle = true }) => (
  <Box displayProp="grid" style={{ gridGap: sectionGap }}>
    {withTitle && <SectionTitle />}
    <Setting width={width1} />
    <Setting width={width2} />
  </Box>
);

const SettingsTabs = () => (
  <Box
    displayProp="grid"
    style={{
      gridGap: "20px",
      gridTemplateColumns: "41px 58px",
    }}
  >
    <RectangleLoader
      height={32}
      backgroundColor={LoaderStyle.backgroundColor}
      foregroundColor={LoaderStyle.foregroundColor}
      backgroundOpacity={LoaderStyle.backgroundOpacity}
      foregroundOpacity={LoaderStyle.foregroundOpacity}
      speed={speed}
      animate={true}
    />
    <RectangleLoader
      height={32}
      backgroundColor={LoaderStyle.backgroundColor}
      foregroundColor={LoaderStyle.foregroundColor}
      backgroundOpacity={LoaderStyle.backgroundOpacity}
      foregroundOpacity={LoaderStyle.foregroundOpacity}
      speed={speed}
      animate={true}
    />
  </Box>
);

const SettingsCommonLoader = ({ isAdmin = false }) => (
  <Box
    widthProp="100%"
    heightProp="100%"
    displayProp="grid"
    style={{ gridGap: sectionsGap }}
  >
    {isAdmin && <SettingsTabs />}
    <SettingsSection width1={"281px"} width2={"351px"} withTitle={!isAdmin} />
    <SettingsSection width1={"103px"} width2={"89px"} />
    <SettingsSection width1={"650px"} width2={"248px"} />
  </Box>
);

export default SettingsCommonLoader;
