import React from "react";
import { observer } from "mobx-react";
import { useTranslation } from "react-i18next";

import FormStore from "@appserver/studio/src/store/SsoFormStore";
import IconButton from "@appserver/components/icon-button";
import Text from "@appserver/components/text";
import { ContextMenuButton } from "@appserver/components";

import StyledCertificatesTable from "../styled-containers/StyledCertificatesTable";
import { addArguments } from "../../../../utils/addArguments";

const CertificatesTable = ({ prefix }) => {
  const { t } = useTranslation(["SingleSignOn", "Common"]);

  const renderRow = (certificate, index) => {
    const onEditClick = addArguments(
      FormStore.onEditClick,
      certificate,
      prefix
    );
    const onDeleteClick = addArguments(
      FormStore.onDeleteClick,
      certificate.crt,
      prefix
    );

    const contextOptions = [
      {
        key: "edit",
        label: t("Common:EditButton"),
        icon: "static/images/access.edit.react.svg",
        onClick: onEditClick,
      },
      {
        key: "delete",
        label: t("Common:Delete"),
        icon: "static/images/button.trash.react.svg",
        onClick: onDeleteClick,
      },
    ];

    const getOptions = () => contextOptions;

    const getFullDate = (date) => {
      return `${new Date(date).toLocaleDateString()}`;
    };

    return (
      <div key={`certificate-${index}`} className="row">
        <IconButton iconName="/static/images/icons/24/file.svg" />
        <div className="column">
          <div className="column-row">
            <Text fontWeight={600} fontSize="14px" noSelect>
              Self-Signed,{" "}
            </Text>
            <Text color="#a3a9ae" noSelect>
              {getFullDate(certificate.startDate)} -{" "}
              {getFullDate(certificate.expiredDate)}
            </Text>
          </div>
          <div className="column-row">
            <Text color="#a3a9ae" fontSize="12px" fontWeight={600} noSelect>
              {certificate.action}
            </Text>
          </div>
        </div>
        <ContextMenuButton className="context-btn" getData={getOptions} />
      </div>
    );
  };

  return (
    <StyledCertificatesTable>
      <div className="body">
        {FormStore[`${prefix}_certificates`].map(renderRow)}
      </div>
    </StyledCertificatesTable>
  );
};

export default observer(CertificatesTable);
