import React, { useEffect, useState } from "react";
import { observer } from "mobx-react";

import toastr from "@docspace/components/toast/toastr";
import { uploadLogo } from "@docspace/common/utils/whiteLabelHelper";

import CommonWhiteLabel from "client/CommonWhiteLabel";

import { useStore } from "SRC_DIR/store";

const Branding = () => {
  const { brandingStore } = useStore();
  const {
    initStore,
    whiteLabelLogos,
    setWhiteLabelLogos,
    whiteLabelLogoText,
    setWhiteLabelLogoText,
    isEqualLogo,
    isEqualText,
    restoreDefault,
  } = brandingStore;

  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    const fetchData = async () => {
      await initStore();
      setIsLoading(false);
    };

    fetchData();
  }, []);

  const onChangeCompanyName = (e: React.FormEvent<HTMLInputElement>) => {
    setWhiteLabelLogoText(e.currentTarget.value);
  };

  const onChangeLogo = async (e: React.FormEvent<HTMLInputElement>) => {
    const target = e.currentTarget;
    const id = target.id.split("_");
    const index = Number(id[1]);
    const theme = id[2];

    let file = target.files && target.files[0];
    target.value = ""; //hack to be able to reselect a file after a default reset

    const { data } = await uploadLogo(file);

    if (data.Success) {
      const url = data.Message;
      const newArr = whiteLabelLogos;

      if (!newArr) return toastr.error("Undefined Error");
      if (theme === "light") {
        newArr[index - 1].path.light = url;
      } else if (theme === "dark") {
        newArr[index - 1].path.dark = url;
      }
      setWhiteLabelLogos(whiteLabelLogos);
    } else {
      console.error(data.Message);
      toastr.error(data.Message);
    }
  };

  const onRestoreDefault = async () => {
    await restoreDefault();
  };

  return isLoading ? (
    <h1>Loading...</h1>
  ) : (
    <CommonWhiteLabel
      isSettingPaid={true}
      logoTextWhiteLabel={whiteLabelLogoText}
      onChangeCompanyName={onChangeCompanyName}
      onUseTextAsLogo={() => console.log("onUseTextAsLogo")}
      logoUrlsWhiteLabel={whiteLabelLogos}
      onChangeLogo={onChangeLogo}
      onSave={() => console.log("onSave")}
      onRestoreDefault={onRestoreDefault}
      isEqualLogo={isEqualLogo}
      isEqualText={isEqualText}
      isSaving={false}
    />
  );
};

export default observer(Branding);
