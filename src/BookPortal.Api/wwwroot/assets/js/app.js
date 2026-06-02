const fallbackBooks = [
  {
    id: "22c217af-b85f-4e03-a4f9-ffbe432a7137",
    title: "Memorias de uma Biblioteca",
    author: "Coletivo Educacao Aberta",
    description: "Ensaios curtos sobre leitura, escola e formacao de leitores.",
    category: "Educacao",
    format: "E-book",
    language: "Portugues",
    ageRange: "14+",
    coverColor: "#174f7a",
    coverAccent: "#f7c948",
    tags: ["sala-de-aula", "educacao", "ensaio"],
    copiesAvailable: 8,
    isAvailable: true,
    addedAt: new Date().toISOString()
  },
  {
    id: "21e577bf-b81c-42f6-8d7a-19b908d4d503",
    title: "Dom Casmurro",
    author: "Machado de Assis",
    description: "Romance brasileiro sobre memoria, ciume e narracao em primeira pessoa.",
    category: "Literatura Brasileira",
    format: "E-book",
    language: "Portugues",
    ageRange: "14+",
    coverColor: "#5b2333",
    coverAccent: "#f3dfbf",
    tags: ["classico", "romance", "vestibular"],
    copiesAvailable: 12,
    isAvailable: true,
    addedAt: new Date().toISOString()
  },
  {
    id: "afe52e80-7c16-4adf-990a-6f356391dd2d",
    title: "O Alienista",
    author: "Machado de Assis",
    description: "Novela satirica sobre ciencia, poder e normalidade.",
    category: "Literatura Brasileira",
    format: "E-book",
    language: "Portugues",
    ageRange: "12+",
    coverColor: "#31473a",
    coverAccent: "#f2a65a",
    tags: ["classico", "conto", "sala-de-aula"],
    copiesAvailable: 6,
    isAvailable: true,
    addedAt: new Date().toISOString()
  },
  {
    id: "3e0c3dd8-0f43-4f31-bd9e-f5ca0b8128f1",
    title: "Iracema",
    author: "Jose de Alencar",
    description: "Romance indianista que integra o repertorio classico brasileiro.",
    category: "Literatura Brasileira",
    format: "E-book",
    language: "Portugues",
    ageRange: "13+",
    coverColor: "#2e6f40",
    coverAccent: "#f4d35e",
    tags: ["classico", "romance", "sala-de-aula"],
    copiesAvailable: 4,
    isAvailable: true,
    addedAt: new Date().toISOString()
  },
  {
    id: "f246b373-0d0d-4750-b79c-08b3cef34875",
    title: "Ciencias em Campo",
    author: "Lia Nascimento",
    description: "Projetos investigativos para conectar ciencias, territorio e cotidiano.",
    category: "Ciencias",
    format: "PDF",
    language: "Portugues",
    ageRange: "10+",
    coverColor: "#096b72",
    coverAccent: "#b8f2e6",
    tags: ["sala-de-aula", "ciencias", "projetos"],
    copiesAvailable: 10,
    isAvailable: true,
    addedAt: new Date().toISOString()
  },
  {
    id: "83ab1b82-62b8-47a3-9297-25f9ba2f7dd1",
    title: "Historias do Brasil em Fontes",
    author: "Marina Valente",
    description: "Documento, imagem e narrativa para investigar periodos da historia brasileira.",
    category: "Historia",
    format: "PDF",
    language: "Portugues",
    ageRange: "12+",
    coverColor: "#7b3f00",
    coverAccent: "#ffd166",
    tags: ["historia", "sala-de-aula", "fontes"],
    copiesAvailable: 0,
    isAvailable: false,
    addedAt: new Date().toISOString()
  }
];

const state = {
  books: [],
  visibleBooks: [],
  shelves: [],
  loans: [],
  profileIdentifier: ""
};

const elements = {
  bookCount: document.querySelector("#bookCount"),
  availableCount: document.querySelector("#availableCount"),
  categoryCount: document.querySelector("#categoryCount"),
  bookGrid: document.querySelector("#bookGrid"),
  shelfList: document.querySelector("#shelfList"),
  searchForm: document.querySelector("#searchForm"),
  searchInput: document.querySelector("#searchInput"),
  categorySelect: document.querySelector("#categorySelect"),
  formatSelect: document.querySelector("#formatSelect"),
  availableOnly: document.querySelector("#availableOnly"),
  clearFilters: document.querySelector("#clearFilters"),
  bookDialog: document.querySelector("#bookDialog"),
  dialogBody: document.querySelector("#dialogBody"),
  dialogClose: document.querySelector("#dialogClose"),
  loanLookupForm: document.querySelector("#loanLookupForm"),
  loanDocument: document.querySelector("#loanDocument"),
  loanList: document.querySelector("#loanList"),
  template: document.querySelector("#bookCardTemplate"),
  userCreateForm: document.querySelector("#userCreateForm"),
  userName: document.querySelector("#userName"),
  userCpf: document.querySelector("#userCpf"),
  userEmail: document.querySelector("#userEmail"),
  userPassword: document.querySelector("#userPassword"),
  userCreateMessage: document.querySelector("#userCreateMessage"),
  profileLoginForm: document.querySelector("#profileLoginForm"),
  profileIdentifier: document.querySelector("#profileIdentifier"),
  profileMessage: document.querySelector("#profileMessage"),
  profilePanel: document.querySelector("#profilePanel"),
  profileName: document.querySelector("#profileName"),
  profileEmail: document.querySelector("#profileEmail"),
  profileBorrowedBooks: document.querySelector("#profileBorrowedBooks"),
  profilePurchasedBooks: document.querySelector("#profilePurchasedBooks"),
  profileLogout: document.querySelector("#profileLogout"),
  navToggle: document.querySelector(".topnav__toggle"),
  topbar: document.querySelector(".topbar")
};

async function fetchJson(url, options) {
  const response = await fetch(url, {
    headers: { "Content-Type": "application/json" },
    ...options
  });

  if (!response.ok) {
    let message = `Request failed: ${response.status}`;
    try {
      const body = await response.json();
      message = body.error || message;
    } catch {
      // Keep the generic message when the server does not return JSON.
    }

    throw new Error(message);
  }

  return response.json();
}

async function loadInitialData() {
  try {
    const [books, shelves] = await Promise.all([
      fetchJson("/api/catalog"),
      fetchJson("/api/shelves")
    ]);
    state.books = books;
    state.shelves = shelves;
  } catch {
    state.books = fallbackBooks;
    state.shelves = buildFallbackShelves(fallbackBooks);
  }

  state.visibleBooks = [...state.books];
  renderCategories();
  renderStats();
  renderCatalog();
  renderShelves();

  const savedIdentifier = window.localStorage.getItem("libraryProfileIdentifier");
  if (savedIdentifier) {
    elements.profileIdentifier.value = savedIdentifier;
    await loadUserProfile(savedIdentifier);
  }
}

function buildFallbackShelves(books) {
  return [
    {
      name: "Novidades do acervo",
      description: "Titulos adicionados recentemente para leitura digital.",
      books: books.slice(0, 4)
    },
    {
      name: "Classicos essenciais",
      description: "Obras de referencia para repertorio literario.",
      books: books.filter((book) => book.tags.includes("classico"))
    },
    {
      name: "Para usar em sala",
      description: "Selecao pensada para projetos, debates e sequencias didaticas.",
      books: books.filter((book) => book.tags.includes("sala-de-aula"))
    }
  ];
}

function renderStats() {
  const categories = new Set(state.books.map((book) => book.category));
  const available = state.books.filter((book) => book.copiesAvailable > 0);
  elements.bookCount.textContent = `${state.books.length} obras`;
  elements.availableCount.textContent = `${available.length} disponiveis`;
  elements.categoryCount.textContent = `${categories.size} categorias`;
}

function renderCategories() {
  const categories = [...new Set(state.books.map((book) => book.category))].sort();

  for (const category of categories) {
    const option = document.createElement("option");
    option.value = category;
    option.textContent = category;
    elements.categorySelect.append(option);
  }
}

function renderCatalog() {
  elements.bookGrid.replaceChildren();

  if (state.visibleBooks.length === 0) {
    const empty = document.createElement("p");
    empty.className = "empty-state";
    empty.textContent = "Nenhuma obra encontrada para os filtros selecionados.";
    elements.bookGrid.append(empty);
    return;
  }

  for (const book of state.visibleBooks) {
    const fragment = elements.template.content.cloneNode(true);
    const card = fragment.querySelector(".book-card");
    const cover = fragment.querySelector(".book-cover");
    const availability = fragment.querySelector(".availability");
    const details = fragment.querySelector("button");

    cover.style.setProperty("--cover-color", book.coverColor);
    cover.style.setProperty("--cover-accent", book.coverAccent);
    fragment.querySelector(".book-cover__title").textContent = book.title;
    fragment.querySelector(".book-cover__author").textContent = book.author;
    fragment.querySelector(".book-card__category").textContent = book.category;
    fragment.querySelector("h3").textContent = book.title;
    fragment.querySelector(".book-card__author").textContent = book.author;
    fragment.querySelector(".book-card__description").textContent = book.description;
    availability.textContent = book.copiesAvailable > 0
      ? `${book.copiesAvailable} disponiveis`
      : "Indisponivel";
    availability.classList.toggle("is-empty", book.copiesAvailable <= 0);

    details.addEventListener("click", () => openBookDialog(book));
    card.addEventListener("dblclick", () => openBookDialog(book));
    elements.bookGrid.append(fragment);
  }
}

function renderShelves() {
  elements.shelfList.replaceChildren();

  for (const shelf of state.shelves) {
    const section = document.createElement("section");
    section.className = "shelf";

    const title = document.createElement("h3");
    title.textContent = shelf.name;

    const description = document.createElement("p");
    description.textContent = shelf.description;

    const list = document.createElement("div");
    list.className = "shelf__books";

    for (const book of shelf.books) {
      const button = document.createElement("button");
      button.className = "shelf-book";
      button.type = "button";
      button.innerHTML = `<strong></strong><span></span>`;
      button.querySelector("strong").textContent = book.title;
      button.querySelector("span").textContent = `${book.author} • ${book.category}`;
      button.addEventListener("click", () => openBookDialog(book));
      list.append(button);
    }

    section.append(title, description, list);
    elements.shelfList.append(section);
  }
}

function applyFilters() {
  const term = elements.searchInput.value.trim().toLowerCase();
  const category = elements.categorySelect.value;
  const format = elements.formatSelect.value;
  const availableOnly = elements.availableOnly.checked;

  state.visibleBooks = state.books.filter((book) => {
    const haystack = [book.title, book.author, book.description, ...book.tags]
      .join(" ")
      .toLowerCase();

    return (!term || haystack.includes(term)) &&
      (!category || book.category === category) &&
      (!format || book.format === format) &&
      (!availableOnly || book.copiesAvailable > 0);
  });

  renderCatalog();
}

function openBookDialog(book) {
  elements.dialogBody.replaceChildren();

  const layout = document.createElement("div");
  layout.className = "dialog-layout";

  const cover = document.createElement("div");
  cover.className = "book-cover";
  cover.style.setProperty("--cover-color", book.coverColor);
  cover.style.setProperty("--cover-accent", book.coverAccent);

  const coverTitle = document.createElement("span");
  coverTitle.className = "book-cover__title";
  coverTitle.textContent = book.title;

  const coverAuthor = document.createElement("span");
  coverAuthor.className = "book-cover__author";
  coverAuthor.textContent = book.author;
  cover.append(coverTitle, coverAuthor);

  const content = document.createElement("div");
  content.className = "dialog-content";

  const title = document.createElement("h2");
  title.textContent = book.title;

  const author = document.createElement("p");
  author.textContent = book.author;

  const description = document.createElement("p");
  description.textContent = book.description;

  const meta = document.createElement("div");
  meta.className = "meta-list";
  for (const item of [book.category, book.format, book.language, book.ageRange, `${book.copiesAvailable} disponiveis`]) {
    const span = document.createElement("span");
    span.textContent = item;
    meta.append(span);
  }

  const form = buildBorrowForm(book);
  content.append(title, author, description, meta, form);
  layout.append(cover, content);
  elements.dialogBody.append(layout);
  elements.bookDialog.showModal();
}

function buildBorrowForm(book) {
  const form = document.createElement("form");
  form.className = "borrow-form";

  const fields = document.createElement("div");
  fields.className = "form-grid";

  const name = buildField("Nome", "text", "borrowerName");
  const documentField = buildField("Documento", "text", "borrowerDocument");
  documentField.input.value = state.profileIdentifier;
  fields.append(name.label, documentField.label);

  const status = document.createElement("div");
  status.className = "toast";
  status.setAttribute("aria-live", "polite");

  const button = document.createElement("button");
  button.className = "button button--primary";
  button.type = "submit";
  button.textContent = book.copiesAvailable > 0 ? "Reservar" : "Fila de espera";

  const buyButton = document.createElement("button");
  buyButton.className = "button button--ghost";
  buyButton.type = "button";
  buyButton.textContent = "Comprar";

  const actions = document.createElement("div");
  actions.className = "form-grid";
  actions.append(button, buyButton);

  form.append(fields, actions, status);
  form.addEventListener("submit", async (event) => {
    event.preventDefault();
    await borrowBook(book, name.input.value, documentField.input.value, status);
  });
  buyButton.addEventListener("click", async () => {
    await buyBook(book, documentField.input.value, status);
  });

  return form;
}

function buildField(labelText, type, id) {
  const label = document.createElement("label");
  label.className = "field";
  label.setAttribute("for", id);

  const span = document.createElement("span");
  span.textContent = labelText;

  const input = document.createElement("input");
  input.id = id;
  input.type = type;
  input.required = true;

  label.append(span, input);
  return { label, input };
}

async function borrowBook(book, userName, userDocument, status) {
  if (!userName.trim() || !userDocument.trim()) {
    status.textContent = "Preencha nome e documento.";
    return;
  }

  try {
    await fetchJson("/api/loans", {
      method: "POST",
      body: JSON.stringify({
        bookId: book.id,
        userDocument,
        userName
      })
    });
    status.textContent = "Emprestimo registrado.";
    if (state.profileIdentifier) {
      await loadUserProfile(state.profileIdentifier);
    }
  } catch {
    if (book.copiesAvailable <= 0) {
      status.textContent = "Obra indisponivel no preview local.";
      return;
    }

    book.copiesAvailable -= 1;
    book.isAvailable = book.copiesAvailable > 0;
    state.loans.unshift({
      id: crypto.randomUUID(),
      bookId: book.id,
      bookTitle: book.title,
      userDocument,
      borrowedAt: new Date().toISOString(),
      dueAt: new Date(Date.now() + 14 * 24 * 60 * 60 * 1000).toISOString()
    });
    status.textContent = "Emprestimo registrado no preview.";
  }

  renderStats();
  applyFilters();
}

async function buyBook(book, userIdentifier, status) {
  const identifier = userIdentifier.trim() || state.profileIdentifier;
  if (!identifier) {
    status.textContent = "Informe CPF ou email para comprar.";
    return;
  }

  try {
    await fetchJson("/api/purchases", {
      method: "POST",
      body: JSON.stringify({
        bookId: book.id,
        userIdentifier: identifier,
        quantity: 1,
        price: 0
      })
    });

    status.textContent = "Livro adicionado aos comprados.";
    state.profileIdentifier = identifier;
    elements.profileIdentifier.value = identifier;
    await loadUserProfile(identifier);
  } catch (error) {
    status.textContent = error.message || "Nao foi possivel comprar este livro.";
  }
}

async function lookupLoans(documentValue) {
  if (!documentValue.trim()) {
    renderLoans([]);
    return;
  }

  try {
    const loans = await fetchJson(`/api/users/${encodeURIComponent(documentValue.trim())}/loans`);
    renderLoans(loans);
  } catch {
    renderLoans(state.loans.filter((loan) => loan.userDocument === documentValue.trim()));
  }
}

function renderLoans(loans) {
  elements.loanList.replaceChildren();

  if (loans.length === 0) {
    const empty = document.createElement("p");
    empty.className = "empty-state";
    empty.textContent = "Nenhum emprestimo encontrado.";
    elements.loanList.append(empty);
    return;
  }

  for (const loan of loans) {
    const item = document.createElement("article");
    item.className = "loan-item";

    const title = document.createElement("strong");
    title.textContent = loan.bookTitle;

    const due = document.createElement("span");
    due.textContent = `Devolucao: ${formatDate(loan.dueAt)}`;

    item.append(title, due);
    elements.loanList.append(item);
  }
}

async function loadUserProfile(identifier) {
  const normalizedIdentifier = identifier.trim();
  if (!normalizedIdentifier) {
    renderProfileMessage("Informe CPF ou email para acessar o perfil.", true);
    return;
  }

  try {
    await fetchJson("/api/auth/login", {
      method: "POST",
      body: JSON.stringify({ identifier: normalizedIdentifier })
    });

    const profile = await fetchJson(`/api/users/profile?identifier=${encodeURIComponent(normalizedIdentifier)}`);
    state.profileIdentifier = normalizedIdentifier;
    window.localStorage.setItem("libraryProfileIdentifier", normalizedIdentifier);
    renderProfile(profile);
    renderProfileMessage("", false);
  } catch (error) {
    state.profileIdentifier = "";
    elements.profilePanel.hidden = true;
    renderProfileMessage(error.message || "Nao foi possivel acessar o perfil.", true);
  }
}

function renderProfile(profile) {
  elements.profilePanel.hidden = false;
  elements.profileName.textContent = profile.name;
  elements.profileEmail.textContent = `${profile.email} | CPF ${profile.cpf}`;
  renderProfileBookList(elements.profileBorrowedBooks, profile.borrowedBooks, "borrowed");
  renderProfileBookList(elements.profilePurchasedBooks, profile.purchasedBooks, "purchased");
}

function renderProfileBookList(container, books, type) {
  container.replaceChildren();

  if (!books || books.length === 0) {
    const empty = document.createElement("p");
    empty.className = "empty-state";
    empty.textContent = type === "borrowed"
      ? "Nenhum livro emprestado para este usuario."
      : "Nenhum livro comprado para este usuario.";
    container.append(empty);
    return;
  }

  for (const book of books) {
    const item = document.createElement("article");
    item.className = "profile-book";

    const title = document.createElement("strong");
    title.textContent = book.title;

    const author = document.createElement("span");
    author.textContent = `${book.author} | ${book.category} | ${book.format}`;

    const status = document.createElement("span");
    status.textContent = type === "borrowed"
      ? buildBorrowedStatus(book)
      : `Compra: ${book.quantity} unidade(s) | ${formatCurrency(book.price)} | ${formatDate(book.createdAt)}`;

    item.append(title, author, status);
    container.append(item);
  }
}

function buildBorrowedStatus(book) {
  if (book.returnedAt) {
    return `Devolvido em ${formatDate(book.returnedAt)}`;
  }

  return `Emprestado em ${formatDate(book.borrowedAt)} | Devolucao: ${formatDate(book.dueAt)}`;
}

function renderProfileMessage(message, isError) {
  elements.profileMessage.replaceChildren();

  if (!message) {
    return;
  }

  const item = document.createElement("p");
  item.className = "empty-state";
  item.textContent = message;
  item.style.color = isError ? "var(--red)" : "var(--green-dark)";
  elements.profileMessage.append(item);
}

function formatDate(value) {
  return new Intl.DateTimeFormat("pt-BR", {
    day: "2-digit",
    month: "2-digit",
    year: "numeric"
  }).format(new Date(value));
}

function formatCurrency(value) {
  return new Intl.NumberFormat("pt-BR", {
    style: "currency",
    currency: "BRL"
  }).format(value);
}

elements.searchForm.addEventListener("submit", (event) => {
  event.preventDefault();
  applyFilters();
});

for (const control of [elements.categorySelect, elements.formatSelect, elements.availableOnly]) {
  control.addEventListener("change", applyFilters);
}

elements.searchInput.addEventListener("input", () => {
  window.clearTimeout(elements.searchInput.searchTimer);
  elements.searchInput.searchTimer = window.setTimeout(applyFilters, 120);
});

elements.clearFilters.addEventListener("click", () => {
  elements.searchForm.reset();
  applyFilters();
});

elements.dialogClose.addEventListener("click", () => elements.bookDialog.close());

elements.loanLookupForm?.addEventListener("submit", async (event) => {
  event.preventDefault();

  await lookupLoans(elements.loanDocument.value);
});

async function createUser() {
  const payload = {
    name: elements.userName.value.trim(),
    cpf: elements.userCpf.value.trim(),
    email: elements.userEmail.value.trim(),
    password: elements.userPassword.value.trim()
  };

  if (!payload.name || !payload.cpf || !payload.email || !payload.password) {
    elements.userCreateMessage.textContent = "Preencha todos os campos.";
    return;
  }

  try {
    await fetchJson("/api/users", {
      method: "POST",
      body: JSON.stringify(payload)
    });

    elements.userCreateMessage.textContent = "Usuario cadastrado com sucesso.";
    elements.userCreateForm.reset();
    elements.profileIdentifier.value = payload.cpf;
    await loadUserProfile(payload.cpf);
  } catch (error) {
    elements.userCreateMessage.textContent = error.message || "Erro ao cadastrar usuario.";
  }
}

elements.userCreateForm?.addEventListener("submit", async (event) => {
  event.preventDefault();
  await createUser();
});

elements.profileLoginForm?.addEventListener("submit", async (event) => {
  event.preventDefault();
  await loadUserProfile(elements.profileIdentifier.value);
});

elements.profileLogout?.addEventListener("click", () => {
  state.profileIdentifier = "";
  elements.profileIdentifier.value = "";
  elements.profilePanel.hidden = true;
  window.localStorage.removeItem("libraryProfileIdentifier");
  renderProfileMessage("Perfil desconectado.", false);
});

elements.navToggle?.addEventListener("click", () => {
  const expanded = elements.navToggle.getAttribute("aria-expanded") === "true";
  elements.navToggle.setAttribute("aria-expanded", String(!expanded));
  elements.topbar?.classList.toggle("menu-open", !expanded);
});

const navLinks = document.querySelectorAll(".topnav a");
navLinks.forEach((link) => {
  link.addEventListener("click", () => {
    if (elements.topbar?.classList.contains("menu-open")) {
      elements.topbar.classList.remove("menu-open");
      elements.navToggle?.setAttribute("aria-expanded", "false");
    }
  });
});

loadInitialData();

