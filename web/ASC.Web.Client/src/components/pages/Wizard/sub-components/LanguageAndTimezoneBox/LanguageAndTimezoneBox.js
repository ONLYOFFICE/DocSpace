import React from 'react';

import {
  Heading,
  Label,
  Icons,
  ComboBox
} from 'asc-web-components';

import StyledLanguageAndTimezoneBox from './StyledLanguageAndTimezoneBox';

const LanguageAndTimezoneBox = ({
  languages, timeZones, t
}) => {
  
  const optionsLang = languages.map( el => Object.assign({}, {
    key: el,
    label: el
  }));

  const optionsTimeZone = timeZones.map( el => Object.assign({}, {
    key: el,
    label: el
  }))

  return <StyledLanguageAndTimezoneBox>
    <Heading level={2} title="Language and Time Zone Settings" className="header-base timezone">
      {t('languageAndTimezoneBox.heading')}
    </Heading>

    <Label 
      className="lang timezone-lang-label"
      text={t('languageAndTimezoneBox.labelLanguage')}
      title="Language"
      display="block"
      htmlFor="language"
    />
    <Icons.QuestionIcon size="small" className="question-icon"/>
    <ComboBox
      className="comboBox"
      id="language" 
      options={optionsLang}
      selectedOption={{
        key: optionsLang[0].key,
        label: optionsLang[0].label
      }}
      dropDownMaxHeight={200}
      onSelect={option => console.log("selected", option)} />
    
    <Label 
      className="timezone timezone-lang-label"
      text={t('languageAndTimezoneBox.labelTimezone')}
      title="Time Zone"
      display="block"
      htmlFor="time-zone"
    />
    <ComboBox
      className="comboBox"
      id="time-zone" 
      options={optionsTimeZone}
      selectedOption={{
        key: optionsTimeZone[0].key,
        label: optionsTimeZone[0].label
      }}
      dropDownMaxHeight={200}
      onSelect={option => console.log("selected", option)} />
    
  </StyledLanguageAndTimezoneBox>
}

export default LanguageAndTimezoneBox;