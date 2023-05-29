import React, { useEffect, useState } from "react";
import { useTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";

import ModalDialog from "@docspace/components/modal-dialog";
import Text from "@docspace/components/text";
import Button from "@docspace/components/button";
import Textarea from "@docspace/components/textarea";

import ModalDialogContainer from "../ModalDialogContainer";

import { getCrashReport } from "SRC_DIR/helpers/crashReport";

const ReportDialog = (props) => {
  const { t, ready } = useTranslation(["Common"]);
  const { visible, onClose, error, user, version } = props;
  const [description, setDescription] = useState("");

  useEffect(() => {
    const report = getCrashReport(user.id, version, user.cultureName, error);
    console.log(report);
  }, []);

  const onChangeTextareaValue = (e) => {
    setDescription(e.target.value);
  };

  return (
    <ModalDialogContainer
      isLoading={!ready}
      visible={visible}
      onClose={onClose}
      displayType="modal"
    >
      <ModalDialog.Header>{"Error report"}</ModalDialog.Header>
      <ModalDialog.Body>
        <Text className="report-description">description</Text>
        <Textarea
          placeholder={t("RecoverDescribeYourProblemPlaceholder")}
          value={description}
          onChange={onChangeTextareaValue}
          autoFocus
          areaSelect
          heightTextArea={72}
          fontSize={13}
        />
      </ModalDialog.Body>
      <ModalDialog.Footer>
        <Button
          key="SendButton"
          label={t("SendButton")}
          size="normal"
          primary
          scale
        />
        <Button
          key="CancelButton"
          label={t("CancelButton")}
          size="normal"
          scale
          onClick={onClose}
        />
      </ModalDialog.Footer>
    </ModalDialogContainer>
  );
};

export default inject(({ auth }) => {
  const { user } = auth.userStore;
  const { firebaseHelper } = auth.settingsStore;

  return {
    user,
    version: auth.version,
    FirebaseHelper: firebaseHelper,
  };
})(observer(ReportDialog));
