import React from "react";

import Button from "@docspace/components/button";

import api from "@docspace/common/api";
import { initPlugin } from "SRC_DIR/helpers/plugins";

const UploadButton = ({ t, addPlugin }) => {
  const inputPluginElement = React.useRef(null);

  const uploadPlugin = React.useCallback(
    async (files) => {
      if (!files) return;

      let formData = new FormData();

      for (let index in Object.keys(files)) {
        formData.append(files[index].name, files[index]);
      }

      try {
        const plugin = await api.plugins.uploadPlugin(formData);

        if (plugin) {
          initPlugin(plugin, addPlugin);
        }

        // addPlugin(plugin);
      } catch (e) {
        console.log(e);
      }
    },
    [addPlugin]
  );

  const onInput = React.useCallback(
    (e) => {
      uploadPlugin(e.target.files);
      e.target.value = null;
    },
    [uploadPlugin]
  );

  const onUploadPluginClick = React.useCallback(() => {
    inputPluginElement.current.click();
  }, [inputPluginElement.current]);

  return (
    <>
      <Button
        className={"plugins__upload-button"}
        size={"small"}
        label={t("Article:Upload")}
        primary
        onClick={onUploadPluginClick}
      />
      <input
        ref={inputPluginElement}
        id="customPluginInput"
        className="custom-plugin-input"
        type="file"
        accept=".js"
        onInput={onInput}
      />
    </>
  );
};

export default React.memo(UploadButton);
