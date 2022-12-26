import React, { useEffect } from "react";
import { inject, observer } from "mobx-react";
import { useTranslation } from "react-i18next";

import { ChangeUserTypeDialog } from "../dialogs";

const ChangeUserTypeEvent = ({}) => {
  const { t } = useTranslation(["Files"]);

  useEffect(() => {
    setVisible(true);
  }, []);

  return <ChangeUserTypeDialog t={t} />;
};

export default inject(({ dialogsStore }) => {
  const {
    changeUserTypeDialogVisible: visible,
    setChangeUserTypeDialogVisible: setVisible,
  } = dialogsStore;

  return {
    visible,
    setVisible,
  };
})(observer(ChangeUserTypeEvent));
