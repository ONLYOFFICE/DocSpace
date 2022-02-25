import React from "react";
import { observer } from "mobx-react";

import FormStore from "@appserver/studio/src/store/SsoFormStore";
import IconButton from "@appserver/components/icon-button";
import Text from "@appserver/components/text";
import { ContextMenuButton } from "@appserver/components";

import StyledCertificatesTable from "../styled-containers/StyledCertificatesTable";
import { addArguments } from "../../../../utils/addArguments";

const CertificatesTable = ({ t, prefix }) => {
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
        {/*TODO*/}
        {/*what name?*/}
        <div className="column name">
          <IconButton iconName="static/images/icons/24/file.svg" />
          <Text fontWeight={600}>Self-Signed</Text>
        </div>
        <div className="column validity">
          <Text color="#a3a9ae">
            {getFullDate(certificate.startDate)} -{" "}
            {getFullDate(certificate.expiredDate)}
          </Text>
        </div>
        {/*action maybe?*/}
        <div className="column status">
          <Text color="#a3a9ae">{certificate.action}</Text>
          <ContextMenuButton getData={getOptions} />
        </div>
      </div>
    );
  };

  return (
    <StyledCertificatesTable>
      <div className="header">
        <div className="header-cell">
          <Text color="#a3a9ae">{t("CertificateName")}</Text>
        </div>
        <div className="header-cell">
          <Text color="#a3a9ae">{t("Validity")}</Text>
        </div>
        <div className="header-cell">
          <Text color="#a3a9ae">{t("CertificateStatus")}</Text>
        </div>
      </div>

      <div className="body">
        {FormStore[`${prefix}_certificates`].map(renderRow)}
      </div>
    </StyledCertificatesTable>
  );
};

export default observer(CertificatesTable);
