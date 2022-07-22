class TagHandler {
  constructor(tags, setTags) {
    this.tags = tags;
    this.setTags = setTags;
    this.fetchedTags = [
      "Figma",
      "Portal",
      "Design",
      "Images",
      "Fi",
      "Fa",
      "Fo",
      "Fam",
      "Fim",
    ];
  }

  createRandomTagId() {
    return "_" + Math.random().toString(36).substr(2, 9);
  }

  refreshDefaultTag(name) {
    let newTags = [...this.tags].filter((tag) => !tag.isDefault);
    newTags.unshift({
      id: this.createRandomTagId(),
      name,
      isDefault: true,
    });

    this.setTags(newTags);
  }

  addTag(name) {
    let newTags = [...this.tags];
    newTags.push({
      id: this.createRandomTagId(),
      name,
    });

    this.setTags(newTags);
  }

  deleteTag(id) {
    this.setTags(this.tags.filter((tag) => tag.id !== id));
  }
}

export default TagHandler;
