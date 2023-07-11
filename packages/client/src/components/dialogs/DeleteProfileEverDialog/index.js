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
import Link from "@docspace/components/link";
import { inject, observer } from "mobx-react";
import config from "PACKAGE_FILE";
import { combineUrl } from "@docspace/common/utils";
import styled from "styled-components";

const { deleteUser } = api.people; //TODO: Move to action
const { Filter } = api;

const StyledModalDialogContainer = styled(ModalDialogContainer)`
  #modal-dialog {
    width: auto;
    max-width: 520px;
    max-height: none;
  }

  .user-delete {
    line-height: 20px;
    padding-bottom: 16px;
  }

  .text-warning {
    color: #f24724;
    font-size: 16px;
    font-weight: 700;
    line-height: 22px;
  }

  .text-delete-description {
    line-height: 20px;
    padding: 8px 0;
  }

  .reassign-data {
    line-height: 15px;
  }
`;

const DeleteProfileEverDialogComponent = (props) => {
  const {
    user,
    t,
    homepage,
    setFilter,
    onClose,
    tReady,
    visible,
    setDataReassignmentDialogVisible,
    setDeleteProfileDialogVisible,
    userPerformedDeletion,
  } = props;
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

  const onClickDataReassignmentDialog = () => {
    setDataReassignmentDialogVisible(true);
    setDeleteProfileDialogVisible(false);
  };
  return (
    <StyledModalDialogContainer
      isLoading={!tReady}
      visible={visible}
      onClose={onClose}
    >
      <ModalDialog.Header>{t("DeleteUser")}</ModalDialog.Header>
      <ModalDialog.Body>
        <Text className="user-delete">
          <Trans i18nKey="DeleteUserMessage" ns="DeleteProfileEverDialog" t={t}>
            {{ userCaption: t("Common:User") }}
            <strong>{{ user: user.displayName }}</strong>
            will be deleted. This action cannot be undone.
          </Trans>
        </Text>
        <Text className="text-warning">{t("Common:Warning")}!</Text>
        <Text className="text-delete-description">
          <Trans
            i18nKey="DeleteReassignDescription"
            ns="DeleteProfileEverDialog"
            t={t}
          >
            All personal files and folders in My documents section of this user
            will be permanently deleted. Rooms created by this user and
            documents stored in these rooms will be reassigned automatically to
            an admin performing the deletion:
            <strong>
              {{ userPerformedDeletion }} ({{ userYou: t("Common:You") }})
            </strong>
            Reassign data manually to choose another destination user for
            reassignment
          </Trans>
        </Text>

        <Link
          className="reassign-data"
          type="action"
          fontSize="13px"
          fontWeight={600}
          isHovered={true}
          onClick={onClickDataReassignmentDialog}
        >
          {t("DataReassignmentDialog:ReassignData")}
        </Link>
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
    </StyledModalDialogContainer>
  );
};

const DeleteProfileEverDialog = withTranslation([
  "DeleteProfileEverDialog",
  "DataReassignmentDialog",
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
  setDataReassignmentDialogVisible:
    peopleStore.dialogStore.setDataReassignmentDialogVisible,
  setDeleteProfileDialogVisible:
    peopleStore.dialogStore.setDeleteProfileDialogVisible,
  userPerformedDeletion: peopleStore.authStore.userStore.user.displayName,
}))(observer(DeleteProfileEverDialog));
