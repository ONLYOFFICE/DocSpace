import React, { useCallback, useState } from 'react';
import PropTypes from "prop-types";
import {
  Button,
  TextInput,
  Text,
  InputBlock,
  Icons,
  SelectedItem
} from "asc-web-components";
import { useTranslation } from 'react-i18next';
import { department, headOfDepartment, typeUser } from '../../../../../helpers/customNames';

const SectionBodyContent = (props) => {
  const { history, group } = props;
  const [value, setValue] = useState(group ? group.name : "");
  const [error, setError] = useState(null);
  const [inLoading, setInLoading] = useState(false);
  const { t } = useTranslation();

  const groupMembers = group && group.members ? group.members : [];

  const onCancel = useCallback(() => {
    history.goBack();
  }, [history]);

  console.log("Group render", props);

  return (
    <>
      <div>
        <label htmlFor="group-name">
          <Text.Body as="span" isBold={true}>{t('CustomDepartmentName', { department })}:</Text.Body>
        </label>
        <div style={{width: "320px"}}>
          <TextInput id="group-name" name="group-name" scale={true} value={value} onChange={(e) => setValue(e.target.value)} />
        </div>
      </div>
      <div style={{ marginTop: "16px" }}>
        <label htmlFor="head-selector">
          <Text.Body as="span" isBold={true}>{t('CustomHeadOfDepartment', { headOfDepartment })}:</Text.Body>
        </label>
        <InputBlock
          id="head-selector"
          value={t('CustomAddEmployee', { typeUser })}
          iconName="ExpanderDownIcon"
          iconSize={8}
          isIconFill={true}
          iconColor="#A3A9AE"
          scale={false}
          isReadOnly={true}
        >
          <Icons.CatalogEmployeeIcon size="medium" />
        </InputBlock>
      </div>
      <div style={{ marginTop: "16px" }}>
        <label htmlFor="employee-selector">
          <Text.Body as="span" isBold={true}>Members:</Text.Body>
        </label>
        <InputBlock
          id="employee-selector"
          value={t('CustomAddEmployee', { typeUser })}
          iconName="ExpanderDownIcon"
          iconSize={8}
          isIconFill={true}
          iconColor="#A3A9AE"
          scale={false}
          isReadOnly={true}
        >
          <Icons.CatalogGuestIcon size="medium" />
        </InputBlock>
      </div>
      <div style={{ marginTop: "16px", display: "flex", flexWrap: "wrap", flexDirection: "row" }}>
        {groupMembers.map(member => 
          <SelectedItem
            text={member.displayName}
            onClick={(e) => console.log("onClose", e.target)}
            isInline={true}
            style={{ marginRight: "8px", marginBottom: "8px" }}
          />
        )}
      </div>
      <div>{error && <strong>{error}</strong>}</div>
      <div style={{ marginTop: "60px" }}>
        <Button label={t('SaveButton')} primary type="submit" isDisabled={inLoading} size="big" />
        <Button
          label={t('CancelButton')}
          style={{ marginLeft: "8px" }}
          size="big"
          isDisabled={inLoading}
          onClick={onCancel}
        />
      </div>
    </>
  );
};

SectionBodyContent.propTypes = {
  group: PropTypes.object
};

SectionBodyContent.defaultProps = {
  group: null
}

export default SectionBodyContent;