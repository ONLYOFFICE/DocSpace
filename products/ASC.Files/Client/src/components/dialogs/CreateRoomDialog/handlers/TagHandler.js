class TagHandler {
  constructor(tags, setTags) {
    this.tags = tags;
    this.setTags = setTags;
  }

  createRandomTagId() {
    return "_" + Math.random().toString(36).substr(2, 9);
  }

  addDefaultTag(name) {
    this.setTags([
      {
        id: this.createRandomTagId(),
        name,
        isDefault: true,
      },
      ...this.tags,
    ]);
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
    this.setTags(
      this.tags.push({ id: this.createRandomTagId(), name, isDefault: false })
    );
  }

  deleteTag(id) {
    this.setTags(this.tags.filter((tag) => tag.id !== id));
  }
}

export default TagHandler;
