import React from "react";
import { useNavigate } from "react-router-dom";
import PropTypes from "prop-types";

import Button from "@docspace/components/button";
import ModalDialog from "@docspace/components/modal-dialog";
import Text from "@docspace/components/text";

import { withTranslation, Trans } from "react-i18next";
import api from "@docspace/common/api";
import toastr from "@docspace/components/toast/toastr";
import ModalDialogContainer from "../ModalDialogContainer";
import { inject, observer } from "mobx-react";
import config from "PACKAGE_FILE";
import { combineUrl } from "@docspace/common/utils";

const { deleteUser } = api.people; //TODO: Move to action
const { Filter } = api;

const DeleteProfileEverDialogComponent = (props) => {
  const { user, t, homepage, setFilter, onClose, tReady, visible } = props;
  const [isRequestRunning, setIsRequestRunning] = React.useState(false);

  const navigate = useNavigate();

  const onDeleteProfileEver = () => {
    const filter = Filter.getDefault();
    const params = filter.toUrlParams();

    const url = `/accounts/filter?${params}`;

    setIsRequestRunning(true);
    deleteUser(user.id)
      .then((res) => {
        toastr.success(t("SuccessfullyDeleteUserInfoMessage"));
        navigate(url, params);
        setFilter(filter);
        return;
      })
      .catch((error) => toastr.error(error))
      .finally(() => onClose());
  };

  return (
    <ModalDialogContainer
      isLoading={!tReady}
      visible={visible}
      onClose={onClose}
    >
      <ModalDialog.Header>{t("DeleteUser")}</ModalDialog.Header>
      <ModalDialog.Body>
        <Text>
          <Trans i18nKey="DeleteUserMessage" ns="DeleteProfileEverDialog" t={t}>
            {{ userCaption: t("Common:User") }}{" "}
            <strong>{{ user: user.displayName }}</strong>
            will be deleted. User personal documents which are available to
            others will be deleted.
          </Trans>
        </Text>
      </ModalDialog.Body>
      <ModalDialog.Footer>
        <Button
          key="OKBtn"
          label={t("Common:Delete")}
          size="normal"
          primary={true}
          scale
          onClick={onDeleteProfileEver}
          isLoading={isRequestRunning}
        />
        <Button
          label={t("Common:CancelButton")}
          size="normal"
          scale
          onClick={onClose}
        />
      </ModalDialog.Footer>
    </ModalDialogContainer>
  );
};

const DeleteProfileEverDialog = withTranslation([
  "DeleteProfileEverDialog",
  "Common",
  "PeopleTranslations",
])(DeleteProfileEverDialogComponent);

DeleteProfileEverDialog.propTypes = {
  visible: PropTypes.bool.isRequired,
  onClose: PropTypes.func.isRequired,
  user: PropTypes.object.isRequired,
};

export default inject(({ peopleStore }) => ({
  homepage: config.homepage,
  setFilter: peopleStore.filterStore.setFilterParams,
}))(observer(DeleteProfileEverDialog));
