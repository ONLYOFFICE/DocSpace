import React from "react";

import Text from "@docspace/components/text";
import Link from "@docspace/components/link";

import EmptyScreenPluginsUrl from "PUBLIC_DIR/images/empty_screen_plugins.svg?url";
import EmptyScreenPluginsDarkUrl from "PUBLIC_DIR/images/empty_screen_plugins_dark.svg?url";

import EmptyFolderContainer from "SRC_DIR/components/EmptyContainer/EmptyContainer";

import UploadButton from "./button";

const EmptyScreen = ({
  t,
  onAddAction,
  theme,
  currentColorScheme,
  learnMoreLink,
  withUpload,
}) => {
  const imageSrc = theme.isBase
    ? EmptyScreenPluginsUrl
    : EmptyScreenPluginsDarkUrl;

  return (
    <EmptyFolderContainer
      headerText={t("NoPlugins")}
      descriptionText={
        <Text>
          {t("Description")}{" "}
          <Link
            color={currentColorScheme?.main?.accent}
            type={"page"}
            target={"_blank"}
            href={learnMoreLink}
          >
            {t("Common:LearnMore")}
          </Link>
        </Text>
      }
      style={{ gridColumnGap: "39px" }}
      buttonStyle={{ marginTop: "16px" }}
      imageSrc={imageSrc}
      buttons={withUpload && <UploadButton t={t} addPlugin={onAddAction} />}
    />
  );
};

export default EmptyScreen;
