# Contribution Guidelines

## Table of Contents
1. [Commit Message Format](#commit-message-format)
   - [Commit Message Types](#commit-message-types)
   - [Examples](#example-commit-messages)
2. [Creating Pull Requests](#creating-pull-requests)
3. [Branching Strategy](#branching)
4. [Naming Conventions](#naming-conventions)
5. [Review and Feedback](#review-and-feedback)

---

## Commit Message Format

We follow a standard format for our commit messages to ensure clarity and ease of understanding. This helps in tracking changes, understanding the purpose of each commit, and maintaining a clean history.

### Structure

commit message structure:

```
<type>(<scope>): <short description>
```

### Commit Message Types

- **feat**: A new feature for the project.
- **fix**: A bug fix or issue resolution.
- **docs**: Documentation changes or additions.
- **style**: Code style changes (e.g., formatting, removing whitespaces).
- **refactor**: Code changes that neither add a feature nor fix a bug (e.g., code reorganisation).
- **test**: Adding missing tests or correcting existing tests.
- **chore**: Changes to build process, auxiliary tools, or project structure.

### Examples

#### 1. Feature (`feat`)

```
feat(player-movement): add basic player movement script using WASD keys
feat(battle-system): implement turn-based combat logic
```

#### 2. Fix (`fix`)

```
fix(enemy-spawn): resolve issue with enemy spawning at incorrect positions
fix(ui-healthbar): prevent health bar from exceeding maximum limit
```

#### 3. Documentation (`docs`)

```
docs(readme): update project setup instructions for new members
docs(contributing): add contribution guidelines for commit message format
```

#### 4. Style (`style`)

```
style(player-controller): reformat script to follow coding standards
style(ui-buttons): adjust spacing and alignment of buttons in the main menu
```

#### 5. Refactor (`refactor`)

```
refactor(battle-system): reorganize battle logic into separate functions
refactor(enemy-ai): extract attack patterns into a separate class
```
---

### Creating Pull Requests

1. **Create a Pull Request**: Create a pull request (PR) from your branch to the `main` branch of the repository.
2. **Title and Description**: Use a clear and descriptive title for your PR. Include a detailed description of the changes and what they address.
3. **Link Issues**: If your PR addresses an issue, link it by including `Fixes #issue_number` in the description.
4. **Request Review**: Assign the PR to the relevant reviewer(s) for approval.

---

## Branching

We use a branching strategy to organise our work:

- `main`: The stable version of the code. All reviewed and tested code is merged into `main`.
- `development`: The primary development branch where new features and bug fixes are integrated.
- `feat/<feature-name>`: Feature branches used to develop new features.
- `fix/<issue-name>`: Bug fix branches to resolve specific issues.

**Example Branch Names:**

- `feat/player-movement`
- `fix/ui-alignment`

---

## Naming Conventions:

- **Variables:** Use `camelCase` for variable names (e.g., `playerHealth`, `enemyCount`).
- **Functions:** Use descriptive function names (e.g., `calculateDamage`, `spawnEnemies`).
- **Classes:** Use `PascalCase` for class names (e.g., `PlayerController`, `EnemyAI`).

---

## Review and Feedback

All contributions will go through a **code review process**. During the review, the reviewer will provide feedback and suggestions. Please ensure that you:

- Reviwer will test the new feature or bug fix ensure it is working on their computer as well and then approve it or reject it if that is not the case 
- The person who requested the pull must then check all comments and changes requested by the reviewer.
- Make necessary adjustments and re-submit for review if required.

---
