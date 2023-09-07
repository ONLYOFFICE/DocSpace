import { useState, useEffect, useCallback } from "react";
import styled from "styled-components";
import { inject, observer } from "mobx-react";
import { withTranslation } from "react-i18next";
import PeopleSelector from "@docspace/client/src/components/PeopleSelector";
import ModalDialog from "@docspace/components/modal-dialog";
import toastr from "@docspace/components/toast/toastr";
import { useNavigate } from "react-router-dom";
import Body from "./sub-components/Body";
import Footer from "./sub-components/Footer";
import api from "@docspace/common/api";
const { Filter } = api;

const StyledModalDialog = styled(ModalDialog)`
  .avatar-name,
  .delete-profile-container {
    display: flex;
    align-items: center;
  }

  .delete-profile-checkbox {
    margin-bottom: 16px;
  }

  .list-container {
    gap: 6px;
  }
`;

let timerId = null;

const DataReassignmentDialog = ({
  visible,
  user,
  setDataReassignmentDialogVisible,
  dataReassignment,
  dataReassignmentProgress,
  currentColorScheme,
  idCurrentUser,
  deleteProfile,
  deleteAdministrator,
  t,
  tReady,
  setFilter,
  setDataReassignmentDeleteAdministrator,
}) => {
  const [selectorVisible, setSelectorVisible] = useState(false);

  if (deleteAdministrator)
    deleteAdministrator.label = deleteAdministrator.displayName;

  const [selectedUser, setSelectedUser] = useState(deleteAdministrator);
  const [isLoadingReassign, setIsLoadingReassign] = useState(false);
  const [isDeleteProfile, setIsDeleteProfile] = useState(deleteProfile);
  const [showProgress, setShowProgress] = useState(false);
  const [isReassignCurrentUser, setIsReassignCurrentUser] = useState(false);

  const [percent, setPercent] = useState(0);

  const navigate = useNavigate();

  const updateAccountsAfterDeleteUser = () => {
    const filter = Filter.getDefault();
    const params = filter.toUrlParams();
    const url = `/accounts/filter?${params}`;

    navigate(url, params);
    setFilter(filter);
    return;
  };

  useEffect(() => {
    //If click Delete user
    if (deleteAdministrator) onReassign();

    return () => setDataReassignmentDeleteAdministrator(null);
  }, [deleteAdministrator]);

  const onToggleDeleteProfile = () => {
    setIsDeleteProfile((remove) => !remove);
  };

  const onTogglePeopleSelector = () => {
    setSelectorVisible((show) => !show);
  };

  const onClose = () => {
    setDataReassignmentDialogVisible(false);
  };

  const onAccept = (item) => {
    setSelectorVisible(false);
    setSelectedUser({ ...item[0] });
  };

  const checkReassignCurrentUser = () => {
    setIsReassignCurrentUser(idCurrentUser === selectedUser.id);
  };

  const checkProgress = (id) => {
    dataReassignmentProgress(id)
      .then((res) => {
        setPercent(res.percentage);

        if (res.percentage !== 100) return;

        clearInterval(timerId);
        isDeleteProfile && updateAccountsAfterDeleteUser();
      })
      .catch((error) => {
        toastr.error(error?.response?.data?.error?.message);
      });
  };

  const onReassign = useCallback(() => {
    checkReassignCurrentUser();
    setIsLoadingReassign(true);
    setShowProgress(true);

    dataReassignment(user.id, selectedUser.id, isDeleteProfile)
      .then(() => {
        toastr.success(t("Common:ChangesSavedSuccessfully"));

        timerId = setInterval(() => checkProgress(user.id), 1000);
      })
      .catch((error) => {
        toastr.error(error?.response?.data?.error?.message);
      });

    setIsLoadingReassign(false);
  }, [user, selectedUser, isDeleteProfile]);

  return (
    <StyledModalDialog
      displayType="aside"
      visible={visible}
      onClose={onClose}
      containerVisible={selectorVisible}
      withFooterBorder
      withBodyScroll
    >
      {selectorVisible && (
        <ModalDialog.Container>
          <PeopleSelector
            acceptButtonLabel={t("Common:SelectAction")}
            excludeItems={[user.id]}
            onAccept={onAccept}
            onCancel={onClose}
            onBackClick={onTogglePeopleSelector}
            withCancelButton
            withAbilityCreateRoomUsers
          />
        </ModalDialog.Container>
      )}

      <ModalDialog.Header>
        {t("DataReassignmentDialog:DataReassignment")}
      </ModalDialog.Header>
      <ModalDialog.Body>
        <Body
          t={t}
          tReady={tReady}
          showProgress={showProgress}
          isReassignCurrentUser={isReassignCurrentUser}
          user={user}
          selectedUser={selectedUser}
          percent={percent}
          currentColorScheme={currentColorScheme}
          onTogglePeopleSelector={onTogglePeopleSelector}
        />
      </ModalDialog.Body>

      <ModalDialog.Footer>
        <Footer
          t={t}
          showProgress={showProgress}
          isDeleteProfile={isDeleteProfile}
          onToggleDeleteProfile={onToggleDeleteProfile}
          selectedUser={selectedUser}
          onReassign={onReassign}
          isLoadingReassign={isLoadingReassign}
          percent={percent}
          onClose={onClose}
        />
      </ModalDialog.Footer>
    </StyledModalDialog>
  );
};

export default inject(({ auth, peopleStore, setup }) => {
  const {
    setDataReassignmentDialogVisible,
    dataReassignmentDeleteProfile,
    dataReassignmentDeleteAdministrator,
    setDataReassignmentDeleteAdministrator,
  } = peopleStore.dialogStore;
  const { currentColorScheme } = auth.settingsStore;
  const { dataReassignment, dataReassignmentProgress } = setup;

  const { id: idCurrentUser } = peopleStore.authStore.userStore.user;

  const { setFilterParams: setFilter } = peopleStore.filterStore;

  return {
    setDataReassignmentDialogVisible,
    theme: auth.settingsStore.theme,
    currentColorScheme,
    dataReassignment,
    idCurrentUser,
    dataReassignmentProgress,
    deleteProfile: dataReassignmentDeleteProfile,
    setFilter,
    deleteAdministrator: dataReassignmentDeleteAdministrator,
    setDataReassignmentDeleteAdministrator,
  };
})(
  observer(
    withTranslation([
      "Common",
      "DataReassignmentDialog",
      "Translations",
      "ChangePortalOwner",
    ])(DataReassignmentDialog)
  )
);
